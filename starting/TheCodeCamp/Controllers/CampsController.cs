using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace TheCodeCamp.Controllers
{
    public class CampsController:ApiController
    {
        public IHttpActionResult Get()
        {
          
            return Ok(new {Name = "Lajos", Occupation = "Developer"});
        }
    }
}