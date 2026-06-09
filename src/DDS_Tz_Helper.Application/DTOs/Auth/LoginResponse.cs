namespace DDS_Tz_Helper.Application.DTOs.Auth;

public class LoginResponse
{
    public string msg { get; set; }
    public string excptn { get; set; }
    public string username { get; set; }
    public int user_id { get; set; }
    public int role_id { get; set; }
    public string user_status { get; set; }
    public int active { get; set; }
    public bool admin { get; set; }
    public bool can_approve { get; set; }
    public string canApproveTare { get; set; }
    public string currentPlant { get; set; }
    public string company_name { get; set; }
    public string loginStatus { get; set; }
    public string token { get; set; }
    public DateTime? tokenExpiresAt { get; set; }
    public bool status { get; set; }
}
