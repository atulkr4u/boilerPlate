using System;
namespace boilerPlate.Domain.DomainBase
{
	public class BaseResponse
	{
		public bool IsSuccess { get; set; }
        public string ReturnMessage { get; set; }
		public string ExceptionMessage { get; set; }
    }
}

