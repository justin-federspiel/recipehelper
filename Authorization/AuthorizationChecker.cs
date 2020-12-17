using System;
using System.Collections.Generic;
using Authentication;
using APICallHandler;

namespace Authorization
{
    public static class AuthorizationChecker
    {
        public static bool CanPerform(APIAction action, string objectType, long objectId, AuthenticationToken user)
        { // check whether the user is allowed to perform action on a(n) (database) object with Type objectType and Id objectId
            return (action == APIAction.GET || action == APIAction.NONE || user != null); //for now, anyone "logged in" can do everything
        }

        public static bool CanPerform(APIAction action, string objectType, IEnumerable<long> objectIds, AuthenticationToken user)
        { // check whether the user is allowed to perform action on the (database) object(s) with Type objectType and Ids objectIds
            return (action == APIAction.GET || action == APIAction.NONE || user != null); //for now, anyone "logged in" can do everything
        }
    }
}
