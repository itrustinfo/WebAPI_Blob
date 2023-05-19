using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Cors;
using System.Security.Claims;
using System.Data;
using PMTWebAPI.DAL;

namespace PMTWebAPI
{
    public class APIAUTHORIZATIONSERVERPROVIDER : OAuthAuthorizationServerProvider
    {
        DBGetData db = new DBGetData();
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated(); //   
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            var httpRequest = HttpContext.Current.Request;
            var UserName = httpRequest.Params["UserName"];
            var Password = httpRequest.Params["Password"];
            //if (context.UserName == "admin" && context.Password == "admin")
            //{
            //    identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
            //    identity.AddClaim(new Claim("username", "admin"));
            //    identity.AddClaim(new Claim(ClaimTypes.Name, "Hi Admin"));
            //    context.Validated(identity);
            //}
            //else
            int sresult = db.checkWebAPIUser(context.UserName,Security.Encrypt(context.Password));
            
           if (sresult > 0)
           {
                identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                identity.AddClaim(new Claim("username", UserName));
                identity.AddClaim(new Claim(ClaimTypes.Name, UserName));
              //  identity.AddClaim(new Claim(ClaimTypes.Expiration, "1234567"));
                context.Validated(identity);
            }
            //else if (context.UserName == "user" && context.Password == "user")
            //{
            //    identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
            //    identity.AddClaim(new Claim("username", "user"));
            //    identity.AddClaim(new Claim(ClaimTypes.Name, "Hi User"));
            //    context.Validated(identity);
            //}
            else
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect");
                return;
            }
        }
    }
}