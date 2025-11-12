using IICUTechServiceReference;
using IWSDLPublishServiceReference;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using test_proxy_backend.DTOs;

namespace test_proxy_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IICUTechController : ControllerBase
    {
        private readonly ILogger<IICUTechController> _logger;
        private readonly IICUTech _iicuTechClient;
        private readonly IWSDLPublish _iicuPublishClient;

        public IICUTechController(ILogger<IICUTechController> logger, IICUTech iicuTechClient, IWSDLPublish wSDLPublish)
        {
            _logger = logger;
            _iicuTechClient = iicuTechClient;
            _iicuPublishClient = wSDLPublish;
        }

        [HttpGet("/version", Name = "GetVersion")]
        public async Task<ActionResult<GetVersionResponseBody>> GetVersion()
        {
            try
            {
                var request = new GetVersionRequest();
                var soapResponse = await _iicuTechClient.GetVersionAsync(request);

                var stringResult = soapResponse.@return; 

                if (string.IsNullOrEmpty(stringResult))
                {
                    _logger.LogWarning("SOAP GetVersion returned empty response.");
                    return Ok(new GetVersionResponseBody { Version = "-1" });
                }

                var versionResult = new GetVersionResponseBody { Version = stringResult };

                return Ok(versionResult); // 200 OK
            }
            catch (System.ServiceModel.FaultException faultEx)
            {
                // SOAP Fault
                _logger.LogError(faultEx, "SOAP Fault in GetVersion: {FaultMessage}", faultEx.Message);
                return StatusCode(503, new GetVersionResponseBody { Version = "-1", Error = faultEx.Message });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Internal server error in GetVersion");
                return StatusCode(500, new GetVersionResponseBody { Version = "-1", Error = e.Message });
            }
        }

        [HttpPost("/login", Name = "Login")]
        public async Task<ActionResult<LoginResponseBody>> PostLogin(LoginRequestBody loginRequestBody)
        {
            try
            {
                var request = new LoginRequest(loginRequestBody.UserName, loginRequestBody.Password, loginRequestBody.IPs);
                var soapResponse = await _iicuTechClient.LoginAsync(request);

                if (string.IsNullOrEmpty(soapResponse.@return))
                {
                    _logger.LogWarning("SOAP GetVersion returned empty response.");
                    return Ok(new LoginResponseBody { ResultCode = -999, ResultMessage = "Error occured while login. Please, try later.", Error = "SOAP GetVersion returned empty response" });
                }

                var LoginResponseDTO = JsonSerializer.Deserialize<LoginResponseBody>(soapResponse.@return, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return Ok(LoginResponseDTO); 

            }
            catch (System.ServiceModel.FaultException faultEx)
            {
                // SOAP Fault 
                _logger.LogError(faultEx, $"SOAP Fault: {loginRequestBody.UserName} {faultEx.Message}");
                return StatusCode(503, new LoginResponseBody { ResultCode = -998, ResultMessage = "Error occured while login. Please, try later.", Error = $"SOAP Fault: {loginRequestBody.UserName} {faultEx.Message}" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Internal server error during login for user {loginRequestBody.UserName}.");
                return StatusCode(500, new LoginResponseBody { ResultCode = -997, ResultMessage = "Error occured while login. Please, try later.", Error = $"Internal server error during login for user {loginRequestBody.UserName}." });
            }
        }
    }
}
