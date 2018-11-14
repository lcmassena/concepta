using System;

namespace Concepta.Services.TravelLogix.Models
{
    public class AuthenticationResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public Int64 expires_in { get; set; }
        public string userName { get; set; }
    }
}
