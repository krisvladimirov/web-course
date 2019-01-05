using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBlog.Data.Seeds;

namespace MyBlog.Authorization.Operations
{
    public class Operations
    {
        public static OperationAuthorizationRequirement Create =
          new OperationAuthorizationRequirement
          {
              Name = Constants.Create
          };
        public static OperationAuthorizationRequirement Read =
          new OperationAuthorizationRequirement
          {
              Name = Constants.Read
          };
        public static OperationAuthorizationRequirement Update =
          new OperationAuthorizationRequirement
          {
              Name = Constants.Update
          };
        public static OperationAuthorizationRequirement Delete =
          new OperationAuthorizationRequirement
          {
              Name = Constants.Delete
          };
    }
}
