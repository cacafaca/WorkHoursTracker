using System.Linq;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System;
using System.DirectoryServices;

namespace ProCode.WorkHoursTracker.Services
{
    public class ActiveDirectory
    {
        private string _firstAndLastName;
        private string _title;
        private string _department;

        public ActiveDirectory()
        {
            Read();
        }

        private string GetProperty(Principal principal, string property)
        {
            DirectoryEntry directoryEntry = principal.GetUnderlyingObject() as DirectoryEntry;
            if (directoryEntry.Properties.Contains(property))
                return directoryEntry.Properties[property].Value.ToString();
            else
                return string.Empty;
        }

        public void Read()
        {
            PrincipalContext domain = new PrincipalContext(ContextType.Domain);
            UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(domain, Environment.UserName);
            _firstAndLastName = GetProperty(userPrincipal, "cn");
            _title = GetProperty(userPrincipal, "title");
            _department = GetProperty(userPrincipal, "department");
        }

        public string FirstAndLastName { get { return _firstAndLastName; } }

        public string Title { get { return _title; } }

        public string Department { get { return _department; } }
    }
}
