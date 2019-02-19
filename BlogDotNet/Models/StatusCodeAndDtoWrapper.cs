using System.Collections.Generic;
using BlogDotNet.Dtos.Responses;
using BlogDotNet.Dtos.Responses.Comment;
using BlogDotNet.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BlogDotNet.Models
{
    public class StatusCodeAndDtoWrapper : ObjectResult
    {
        public AppResponse Dto { get; set; }

        public StatusCodeAndDtoWrapper(int statusCode, AppResponse dto) : base(dto)
        {
            StatusCode = statusCode;
            Dto = dto;
        }

        public StatusCodeAndDtoWrapper(AppResponse dto) : this(200, dto)
        {
        }

        private StatusCodeAndDtoWrapper(int statusCode, AppResponse dto, string message) : base(dto)
        {
            StatusCode = statusCode;
            dto.FullMessages?.Add(message);
        }

        public static IActionResult BuildGenericNotFound()
        {
            return new StatusCodeAndDtoWrapper(404, new ErrorDtoResponse("Not Found"));
        }

        public static StatusCodeAndDtoWrapper BuilBadRequest(ModelStateDictionary modelStateDictionary)
        {
            ErrorDtoResponse errorRes = new ErrorDtoResponse();

            foreach (var key in modelStateDictionary.Keys)
            {
                foreach (var error in modelStateDictionary[key].Errors)
                {
                    errorRes.FullMessages.Add(error.ErrorMessage);
                }
            }

            return new StatusCodeAndDtoWrapper(400, errorRes);
        }

        public static IActionResult BuildSuccess(AppResponse dto)
        {
            return new StatusCodeAndDtoWrapper(200, dto);
        }

        public static IActionResult BuildSuccess(AppResponse dto, string message)
        {
            return new StatusCodeAndDtoWrapper(200, dto, message);
        }

        public static IActionResult BuildSuccess(string updatedSuccessfully)
        {
            return new StatusCodeAndDtoWrapper(200, new SuccessResponse(updatedSuccessfully));
        }

        public static IActionResult BuildErrorResponse(string message)
        {
            return new StatusCodeAndDtoWrapper(500, new ErrorDtoResponse(message));
        }


        public static IActionResult BuildBadRequest(IEnumerable<IdentityError> resultErrors)
        {
            ErrorDtoResponse res = new ErrorDtoResponse();
            foreach (var resultError in resultErrors)
                res.FullMessages.Add(resultError.Description);

            return new StatusCodeAndDtoWrapper(400, res);
        }

        
    }
}