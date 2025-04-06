using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        //for sign up purpose
        [HttpPost]

        public async Task<ActionResult<CreateClientResponse>> CreateClient(CreateClientRequest request)
        {
            try
            {
                var result = await _clientService.CreateClient(request);
                return Created("", result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
   
}
