using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMapper;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    [RoutePrefix("api/camps")]
    public class CampsController:ApiController
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;

        public CampsController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [Route()]
        public async Task<IHttpActionResult> Get(bool includeTalks=false)
        {
            try
            {
                var result = await _repository.GetAllCampsAsync(includeTalks);

                //mapping
                var mappedResult = _mapper.Map<IEnumerable<CampModel>>(result);
                
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                //todo: add logging
                return InternalServerError(ex);
            }
            
        }
        [Route("{moniker}", Name = "GetCamp")]
        public async Task<IHttpActionResult> Get(string moniker,bool includeTalks=false)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker, includeTalks);
                if (result == null) return NotFound();
                return Ok(_mapper.Map<CampModel>(result));

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [Route("searchByDate/{eventDate:dateTime}")]
        [HttpGet]
        public async Task<IHttpActionResult> SearchByEventDate(DateTime eventDate, bool includeTalks=false)
        {
            try
            {
                var result = await _repository.GetAllCampsByEventDate(eventDate, includeTalks);

                return Ok(_mapper.Map<CampModel[]>(result));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [Route()]
        public async Task<IHttpActionResult> Post(CampModel model)
        {
            try
            {
                if (await _repository.GetCampAsync(model.Moniker) != null)
                {
                    ModelState.AddModelError("Moniker", "Moniker is in use!");
                }

                if (ModelState.IsValid)
                {
                   
                    var camp = _mapper.Map<Camp>(model);
                    _repository.AddCamp(camp);
                    if (await _repository.SaveChangesAsync())
                    {
                        var newModel = _mapper.Map<CampModel>(camp);

                        // var location = $"/api/camps/{newModel.Moniker}"; not the best way to do in this way, because if we change the routes later this will end with errors

                        //var location = Url.Link("GetCamp", new {moniker = newModel.Moniker});
                       // return Created(location,newModel);

                       //this is better aproach

                       return CreatedAtRoute("GetCamp", new {moniker = newModel.Moniker}, newModel);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return BadRequest(ModelState);
        }

        [Route("{moniker}")]
        public async Task<IHttpActionResult> Put(string moniker, CampModel model)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null) return NotFound();

                _mapper.Map(model, camp);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok(_mapper.Map<CampModel>(camp));
                }
                else
                {
                    return InternalServerError();
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("{moniker}")]
        public async Task<IHttpActionResult> Delete(string moniker)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null) return NotFound();

                _repository.DeleteCamp(camp);
                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}