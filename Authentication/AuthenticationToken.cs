using System;

namespace Authentication
{
    public class AuthenticationToken
    {
        public string ApplicationWideName { get; set; }

        public long ApplicationWideId { get; set; }   
        public byte[] ThirdPartyProvidedValue { get; set; }

        public AuthenticationToken()
        {
            ApplicationWideName = "Matt Stemm";
            ApplicationWideId = 1;
            ThirdPartyProvidedValue = new byte[0];
        }

        public AuthenticationToken(string something, byte[] three_p_provided = null) 
        {
            ApplicationWideName = something;
            //Create new Application-wide-Id...
            ApplicationWideId = 2;
            if(three_p_provided != null)
            {
                ThirdPartyProvidedValue = three_p_provided;
            } else
            {
                ThirdPartyProvidedValue = new byte[0];
            }
        }        
    }
}
