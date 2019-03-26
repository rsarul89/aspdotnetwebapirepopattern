﻿using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace ApiEFAutofac.Controllers
{
    [RoutePrefix("api/standard")]
    public class StandardController : ApiController
    {
        private readonly IStandardRepository<Standard> _service;
        public StandardController(IStandardRepository<Standard> service)
        {
            _service = service;
        }
        [HttpGet, Route("getAll"), ResponseType(typeof(IEnumerable<Standard>))]
        public IHttpActionResult GetAll()
        {
            IEnumerable<Standard> result = null;
            try
            {
                result = _service.GetAllStandards();
            }
            catch(Exception ex)
            {

            }
            return Ok(result);
        }
    }
}
