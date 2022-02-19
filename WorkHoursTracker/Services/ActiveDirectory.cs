using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ProCode.WorkHoursTracker.Services
{
    public class ActiveDirectory
    {
        #region Fields
        const string _notApplicable = "N/A";
        private string _firstAndLastName;
        private string _title;
        private string _department;
        #endregion

        #region Constructors
        public ActiveDirectory()
        {
            Read();
        }
        #endregion

        #region Properties
        public string FirstAndLastName { get { return _firstAndLastName; } }
        public string Title { get { return _title; } }
        public string Department { get { return _department; } }
        #endregion

        #region Methods
        public void Read()
        {
            try
            {
                PrincipalContext domain = new PrincipalContext(ContextType.Domain);
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(domain, Environment.UserName);
                _firstAndLastName = GetProperty(userPrincipal, "cn");
                _title = GetProperty(userPrincipal, "title");
                _department = GetProperty(userPrincipal, "department");
            }
            catch (PrincipalServerDownException)
            {
                SetNonDomainDefaultValues();
            }
        }

        private string GetProperty(Principal principal, string property)
        {
            DirectoryEntry directoryEntry = principal.GetUnderlyingObject() as DirectoryEntry;
            if (directoryEntry.Properties.Contains(property))
                return directoryEntry.Properties[property].Value.ToString();
            else
                return string.Empty;
        }

        void SetNonDomainDefaultValues()
        {            
            _firstAndLastName = Environment.UserName;
            _title = _notApplicable;
            _department = _notApplicable;
        }
        #endregion
    }
}
