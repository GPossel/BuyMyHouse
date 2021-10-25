using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Enums;
using BuyMyHouse.Domain.DTO;
using BuyMyHouse.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FunctionApp1
{
    public class UserFunctions
    {
        public IUserService _userService { get; set; }

        public UserFunctions(IUserService userService)
        {
            this._userService = userService;
        }

        [Function(nameof(GetUserByIdAndName))]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "Id of user to get", Description = "The id of the user to get", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "firstname", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "firstname of user", Description = "firstname", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "lastname", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "lastname of user", Description = "lastname", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> GetUserByIdAndName([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,  FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            var user = new UserDTO();
            try
            {
                Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(req.Url.Query);
                string userId = query["userId"];
                string firstname = query["firstname"];
                string lastname = query["lastname"];
                user = await _userService.GetEntity(userId, firstname+lastname);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(user);
            return response;
        }

        [Function(nameof(CreateUser))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UserDTO), Required = true, Description = "User object that needs to be added to the database")]
        public async Task<HttpResponseData> CreateUser([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,  FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            var userDTO = new UserDTO();
            try
            {
                string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
                UserDTO user = JsonConvert.DeserializeObject<UserDTO>(requestbody);
                userDTO = await _userService.CreateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(userDTO);
            return response;
        }

        [Function(nameof(UpdateUser))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UserDTO), Required = true, Description = "User to update")]
        public async Task<HttpResponseData> UpdateUser([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            var userDTO = new UserDTO();
            try
            {
                string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
                UserDTO user = JsonConvert.DeserializeObject<UserDTO>(requestbody);
                userDTO = await _userService.UpdateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(userDTO);
            return response;
        }

        [Function(nameof(DeleteUser))]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "Id of user to delete", Description = "The id of the user to delete", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "firstname", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "firstname of user", Description = "firstname", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "lastname", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "lastname of user", Description = "lastname", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> DeleteUser([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            var result = false;
            try
            {
                Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(req.Url.Query);
                string userId = query["userId"];
                string firstname = query["firstname"];
                string lastname = query["lastname"];
                result = await _userService.DeleteUser(userId, firstname+lastname);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(result);
            return response;
        }
    
    }
}
