using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MVP_API
{
    public class SubjectNameRequirement : IAuthorizationRequirement
    {

        public SubjectNameRequirement(string commaSeparatedListOfSubjectNames)
        {
            AuthorizedUsers = commaSeparatedListOfSubjectNames.Split(',').ToList();
        }

        public IEnumerable<string> AuthorizedUsers { get; }
    }

    public class SubjectNameAuthorizationHandler : AuthorizationHandler<SubjectNameRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SubjectNameRequirement requirement)
        {            
            var claimToReview = context.User.Claims.SingleOrDefault(c => c.Type.Contains("nameidentifier"));
            if (claimToReview == null) return Task.CompletedTask;

            if( requirement.AuthorizedUsers.Any(subjectName => string.Equals(subjectName.Trim(), claimToReview.Value.Trim(), StringComparison.OrdinalIgnoreCase)))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}