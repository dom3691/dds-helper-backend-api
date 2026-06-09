using DDS_Tz_Helper.Application.DTOs.ATCDetails;
using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Infrastructure.Services;

public class ATCDetailsService : IATCDetailsService
{
    private readonly IDispatchDBContext _context;
    private readonly ITransactionLogService _transactionLogService;

    public ATCDetailsService(IDispatchDBContext context, ITransactionLogService transactionLogService)
    {
        _context = context;
        _transactionLogService = transactionLogService;
    }

    public async Task<sto_datax> DoFetchATCDetailsAsync(FetchATCDetailsRequest request, CancellationToken cancellationToken = default)
    {
        var response = new sto_datax();

        var poDetails = await _context.sto_transaction_data.FirstOrDefaultAsync(x => x.po_doc_number == request.atc && x.sync_status == false, cancellationToken);
        await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "FETCH_ATC_ATTEMPT_HELPER", "FETCH_ATC_ATTEMPT : ", cancellationToken);

        if (poDetails == null)
        {
            response.status = false;
            response.msg = "Wrong PO or PO already weighed-out and Pushed to SAP";
            await _context.SaveChangesAsync(cancellationToken);
            return response;
        }

        response.msg = "";
        response.status = true;
        response.transporter = poDetails.transporter_name;
        response.trip_id = poDetails.trip_id;
        response.destination = poDetails.destination;
        response.driverName = poDetails.driver;
        response.sender = poDetails.sender;
        response.stoLoc = poDetails.loc;
        response.truckNumber = poDetails.vehicle;
        response.receiver = poDetails.receiver;
        response.atc = request.atc;
        await _context.SaveChangesAsync(cancellationToken);
        return response;
    }

    public async Task<Gate_Datax> DoFetchATCDetailsGateAsync(FetchATCDetailsRequest request, CancellationToken cancellationToken = default)
    {
        var response = new Gate_Datax();

        var atcDetails = await _context.gateaccess_info.FirstOrDefaultAsync(x => x.atc_no == request.atc && x.status != "Weigh-Out Successful." && x.status != "CANCELLED", cancellationToken);
        await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "FETCH_ATC_ATTEMPT_HELPER", "FETCH_ATC_ATTEMPT GATE : ", cancellationToken);

        if (atcDetails == null)
        {
            response.status = false;
            response.msg = "Wrong ATC or ATC already weighed-out";
            await _context.SaveChangesAsync(cancellationToken);
            return response;
        }

        response.msg = "";
        response.status = true;
        response.transporter = atcDetails.transporter;
        response.statusDB = atcDetails.status;
        response.destination = atcDetails.destination;
        response.driverName = atcDetails.driver;
        response.transType = atcDetails.transaction_type;
        response.ShippingPoint = atcDetails.shipping_point;
        response.License = atcDetails.license;
        response.customer = atcDetails.customer;
        response.truckNumber = atcDetails.truck_no;
        response.product = atcDetails.fleet_no;
        response.parentSO = atcDetails.parent_so;
        response.atc = request.atc;
        await _context.SaveChangesAsync(cancellationToken);
        return response;
    }

    public async Task<object> DoFetchATcForArchiveDetailsAsync(FetchATCDetailsRequest request, CancellationToken cancellationToken = default)
    {
        var trimmedATC = request.atc?.TrimStart('0') ?? string.Empty;
        var tradeDetails = await _context.Trade.FirstOrDefaultAsync(a => a.sparenum2.ToString() == trimmedATC && a.seconddatetime == null, cancellationToken);
        if (tradeDetails != null)
        {
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "FETCH_ATC_ATTEMPT_FOR_ARCHIVE_HELPER", "FETCH ATC ATTEMPT FOR ARCHIVE : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return new trade_dto { msg = "", status = true, statusPicking = false, atc = tradeDetails.sparenum2.ToString(), trip_ID = "0000000000" };
        }

        var stoOut = await _context.sto_transaction_data.FirstOrDefaultAsync(x => x.po_doc_number == request.atc && x.gross == null && x.sales_type == "PO_OUT", cancellationToken);
        if (stoOut != null)
        {
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "FETCH_ATC_ATTEMPT_FOR_ARCHIVE_HELPER", "FETCH ATC ATTEMPT FOR ARCHIVE : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return new Sto_Transaction_Datax { msg = "", status = true, atc = stoOut.po_doc_number, trip_id = stoOut.trip_id };
        }

        var stoIn = await _context.sto_transaction_data.FirstOrDefaultAsync(x => x.po_doc_number == request.atc && x.tare == null && x.sales_type == "PO_IN", cancellationToken);
        if (stoIn != null)
        {
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "FETCH_ATC_ATTEMPT_FOR_ARCHIVE_HELPER", "FETCH ATC ATTEMPT FOR ARCHIVE : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return new Sto_Transaction_Datax { msg = "", status = true, atc = stoIn.po_doc_number, trip_id = stoIn.trip_id };
        }

        var atcDetails = await _context.transaction_data.FirstOrDefaultAsync(x => x.sales_doc_number == request.atc && x.gross == null, cancellationToken);
        await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "FETCH_ATC_ATTEMPT_FOR_ARCHIVE_HELPER", "FETCH ATC ATTEMPT FOR ARCHIVE : ", cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        if (atcDetails == null)
        {
            return new Transaction_Datax { status = false, msg = "Wrong ATC or ATC is already weighed-out" };
        }

        return new Transaction_Datax { msg = "", status = true, atc = request.atc, trip_id = atcDetails.trip_id };
    }

    public async Task<object> DoFetchATcForTareChangeDetailsAsync(FetchATCDetailsRequest request, CancellationToken cancellationToken = default)
    {
        var trimmedATC = request.atc?.TrimStart('0') ?? string.Empty;
        var tradeDetails = await _context.Trade.FirstOrDefaultAsync(a => a.sparenum2.ToString() == trimmedATC && a.seconddatetime == null, cancellationToken);
        if (tradeDetails != null)
        {
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "FETCH_ATC_ATTEMPT_FOR_TARE_CHANGE_HELPER", "FETCH ATC ATTEMPT FOR TARE CHANGE : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return new trade_dto
            {
                msg = "",
                status = true,
                statusPicking = false,
                operator_weighn = tradeDetails.username1,
                Tare = tradeDetails.tare.ToString(),
                tare_time = tradeDetails.firstdatetime,
                atc = tradeDetails.sparenum2.ToString()
            };
        }

        var stoOut = await _context.sto_transaction_data.FirstOrDefaultAsync(x => x.po_doc_number == request.atc && x.gross == null && x.sales_type == "PO_OUT", cancellationToken);
        if (stoOut != null)
        {
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "FETCH_ATC_ATTEMPT_FOR_TARE_CHANGE_HELPER", "FETCH ATC ATTEMPT FOR TARE CHANGE : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return new Sto_Transaction_Datax
            {
                msg = "",
                status = true,
                atc = stoOut.po_doc_number,
                tare_time = stoOut.tare_time ?? DateTime.MinValue,
                operator_weighn = stoOut.operator_weighin,
                tare = stoOut.tare.ToString(),
                trip_id = stoOut.trip_id
            };
        }

        var stoIn = await _context.sto_transaction_data.FirstOrDefaultAsync(x => x.po_doc_number == request.atc && x.tare == null && x.sales_type == "PO_IN", cancellationToken);
        if (stoIn != null)
        {
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "FETCH_ATC_ATTEMPT_FOR_TARE_CHANGE_HELPER", "FETCH ATC ATTEMPT FOR TARE CHANGE : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return new Sto_Transaction_Datax
            {
                msg = "",
                status = true,
                atc = stoIn.po_doc_number,
                trip_id = stoIn.trip_id,
                tare_time = stoIn.tare_time ?? DateTime.MinValue,
                operator_weighn = stoIn.operator_weighin,
                tare = stoIn.tare.ToString()
            };
        }

        var atcDetails = await _context.transaction_data.FirstOrDefaultAsync(x => x.sales_doc_number == request.atc && x.gross == null, cancellationToken);
        await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "FETCH_ATC_ATTEMPT_FOR_TARE_CHANGE_HELPER", "FETCH ATC ATTEMPT FOR TARE CHANGE : ", cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        if (atcDetails == null)
        {
            return new Transaction_Datax { status = false, msg = "Wrong ATC or ATC is already weighed-out" };
        }

        return new Transaction_Datax
        {
            msg = "",
            status = true,
            atc = request.atc,
            trip_id = atcDetails.trip_id,
            tare = atcDetails.tare.ToString(),
            operator_weighn = atcDetails.operator_weighin,
            tare_time = atcDetails.tare_time ?? DateTime.MinValue
        };
    }

    public async Task<sto_datax> DoFetchATCDetailsForQuantityUpdateAsync(FetchATCDetailsRequest request, CancellationToken cancellationToken = default)
    {
        var response = new sto_datax();
        var delWeighOut = await _context.sto_transaction_data.FirstOrDefaultAsync(x => x.po_doc_number == request.atc && x.nett != null, cancellationToken);
        if (delWeighOut != null)
        {
            response.msg = "This Delivery Is Weighed Out";
            response.status = false;
            return response;
        }

        var stoDataQty = await _context.sto_data.FirstOrDefaultAsync(x => x.delivery_number == request.atc && x.used != null, cancellationToken);
        if (stoDataQty == null)
        {
            stoDataQty = await _context.sto_data.FirstOrDefaultAsync(x => x.delivery_number_out == request.atc && x.used != null, cancellationToken);
        }

        if (stoDataQty != null)
        {
            response.msg = "";
            response.status = true;
            response.atc = request.atc;
            response.del_qty = stoDataQty.del_qty ?? 0;
            response.Quantity = stoDataQty.del_qty ?? 0;
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "FETCH_ATC_ATTEMPT_FOR_QTY_UPDATE_HELPER", "FETCH ATC ATTEMPT FOR QTY UPDATE : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return response;
    }

    public async Task<sto_datax> DoFetchATcForGrossModifAsync(FetchATCDetailsRequest request, CancellationToken cancellationToken = default)
    {
        var response = new sto_datax();

        var pending = await _context.transaction_data_archive.FirstOrDefaultAsync(x => x.sales_doc_number == request.atc && x.operator_weighin == "ADDITIONAL GROSS WEIGHT MODIF REQUEST", cancellationToken);
        if (pending != null)
        {
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "FETCH_ATC_ATTEMPT_FOR_GROSS_CHANGE_HELPER", "FETCH_ATC_ATTEMPT_FOR GROSS_CHANGE : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            response.status = false;
            response.msg = "Theres A Pending Request For Gross WT Change For This Same ATC!!!";
            return response;
        }

        var transDetails = await _context.transaction_data.FirstOrDefaultAsync(x => x.sales_doc_number == request.atc && x.sync_status == false && x.operator_picking != null && x.gross == null, cancellationToken);
        if (transDetails == null)
        {
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "FETCH_ATC_ATTEMPT_FOR_GROSS_CHANGE_HELPER", "FETCH_ATC_ATTEMPT_FOR_GROSS_CHANGE_HELPER : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            response.status = false;
            response.msg = "Wrong ATC Or ATC Is Weighed Out Or ATC Is Pushed To SAP Already";
            return response;
        }

        var weightChange = await _context.weight_change.FirstOrDefaultAsync(x => x.atc_no == request.atc && x.change_request_id != null && x.status == "APPROVED" && x.net_weight != null && (x.weight_change1 == "-150" || x.weight_change1 == "150"), cancellationToken);
        if (weightChange == null)
        {
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "CHECK_FOR_DDS_APPROVAL_MAX_GROSS_WT_CHANGE", "CHECK_FOR_DDS_APPROVAL_MAX_GROSS_WT_CHANGE : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            response.status = false;
            response.msg = "MAXIMUM Gross Weight Change Needs To Be Requested And Approved From DDS First";
            return response;
        }

        var changeRequest = await _context.change_request.FirstOrDefaultAsync(x => x.id == weightChange.change_request_id && x.request_type == "GROSS_WEIGHT_CHANGE" && x.date_of_approval != null && x.approved == true, cancellationToken);
        if (changeRequest == null)
        {
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "CHECK_FOR_DDS_APPROVAL_MAX_GROSS_WT_CHANGE", "CHECK_FOR_DDS_APPROVAL_MAX_GROSS_WT_CHANGE : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            response.msg = "Please Ensure Initial DDS Approval Request Was Granted";
            response.status = false;
            return response;
        }

        await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "CHECK_FOR_DDS_APPROVAL_MAX_GROSS_WT_CHANGE", "CHECK_FOR_DDS_APPROVAL_MAX_GROSS_WT_CHANGE : ", cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        response.msg = "";
        response.status = true;
        response.grade = weightChange.weight_change1;
        response.stoLoc = weightChange.status;
        response.sender = weightChange.net_weight;
        response.driverName = weightChange.tare_weight;
        response.destination = weightChange.gross_weight;
        response.receiver = changeRequest.notes;
        response.requestedBy = changeRequest.requested_by;
        response.approvedBy = changeRequest.approved_by;
        response.requestType = changeRequest.request_type;
        response.dateOfApproval = changeRequest.date_of_approval ?? DateTime.MinValue;
        response.dateOfRequest = changeRequest.date_of_request ?? DateTime.MinValue;
        response.Quantity = transDetails.quantity ?? 0;
        response.truckNumber = transDetails.vehicle;
        response.TruckDriverName = transDetails.driver;
        response.Migo_Details = "CEMENT";
        response.truckDestination = transDetails.destination;
        response.operatorWeighin = transDetails.operator_weighin;
        response.operatorPicking = transDetails.operator_picking;
        response.atc = request.atc;
        return response;
    }

}

