namespace DDS_Tz_Helper.Application.DTOs.ATCSwap;

public class DoATCSwapRequest
{
    public string atc { get; set; }
    public string newATC { get; set; }
    public string trip_id { get; set; }
    public string reason { get; set; }
    public int user_id { get; set; }
    public bool TrxSwapPicking { get; set; }
}
