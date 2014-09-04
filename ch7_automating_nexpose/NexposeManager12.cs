using System;
using System.Xml;

namespace nexposesharp
{
	public class NexposeManager12 : NexposeManager11, IDisposable
	{
		NexposeSession _session;
		
		public NexposeManager12 (NexposeSession session) : base (session)
		{
			if (!session.IsAuthenticated)
				throw new Exception("Trying to create manager from unauthenticated session. Please authenticate.");
			
			_session = session;
		}
		
		public XmlDocument GetEngineConfig(string engineID)
		{
			string cmd = "<EngineConfigRequest session-id=\"" + _session.SessionID + "\" engine-id=\"" + engineID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;

		}
		
		public XmlDocument DeleteEngine(string engineID)
		{
			string cmd = "<EngineDeleteRequest session-id=\"" + _session.SessionID + "\" engine-id=\"" + engineID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
		}
		
		public XmlDocument SaveOrUpdateEngine(XmlNode engine)
		{
			string cmd = "<EngineSaveRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + engine.OuterXml;
			
			cmd = cmd + "</EngineSaveRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument CreateTicket(XmlNode ticket)
		{
			string cmd = "<TicketCreateRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + ticket.OuterXml;
			cmd = cmd + "</TicketCreateRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument GetTicketListing()
		{
			string cmd = "<TicketListingRequest session-id=\"" + _session.SessionID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument GetTicketDetails(string ticketID)
		{
			string cmd = "<TicketDetailsRequest session-id=\"" + _session.SessionID + "\"  >";
			
			cmd = cmd + "<Ticket id=\"" + ticketID + "\" /></TicketDetailsRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument DeleteTicket(string ticketID)
		{
			string cmd = "<TicketDeleteRequest session-id=\"" + _session.SessionID + "\" >";
			cmd = cmd + "<Ticket id=\"" + ticketID + "\" /></TicketDeleteRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument GetPendingVulnExceptionCount()
		{
			string cmd = "<PendingVulnExceptionCountRequest session-id=\"" + _session.SessionID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
		}
		
		public XmlDocument GetVulnerabilityExceptionListing()
		{
			string cmd = "<VulnerabilityExceptionListingRequest session-id=\"" + _session.SessionID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument CreateVulnerabilityException()
		{
			string cmd = "<VulnerabilityExceptionCreateRequest session-id=\"" + _session.SessionID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument ResubmitVulnerabilityException(string vulnExceptionID, string comment)
		{
			string cmd = "<VulnerabilityExceptionReSubmitRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + "<comment>" + comment + "</comment></VulnerabilityExceptionReSubmitRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument RecallVulnerabilityException(string vulnExceptionID)
		{
			string cmd = "<VulnerabilityExceptionRecallRequest session-id=\"" + _session.SessionID + "\" exception-id=\"" + vulnExceptionID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument ApproveVulnerabilityException(string vulnExceptionID)
		{
			string cmd = "<VulnerabilityExceptionApproveRequest session-id=\"" + _session.SessionID + "\" exception-id=\"" + vulnExceptionID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument RejectVulnerabilityException(string vulnExceptionID)
		{
			string cmd = "<VulnerabilityExceptionRejectRequest session-id=\"" + _session.SessionID + "\"  exception-id=\"" + vulnExceptionID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument DeleteVulnerabilityException(string vulnExceptionID)
		{
			string cmd = "<VulnerabilityExceptionDeleteRequest session-id=\"" + _session.SessionID + "\"  exception-id=\"" + vulnExceptionID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument UpdateCommentForVulnerabilityException(string vulnExceptionID, string reviewerComment, string submitterComment)
		{
			string cmd = "<VulnerabilityExceptionUpdateCommentRequest session-id=\"" + _session.SessionID + "\" exception-id=\"" + vulnExceptionID + "\" >";
			
			cmd = cmd + "<reviewer-comment>" + CleanString(reviewerComment) + "</reviewer-comment>";
			cmd = cmd + "<submitter-comment>" + CleanString(submitterComment) + "</submitter-comment>";
			cmd = cmd + "</VulnerabilityExceptionUpdateCommentRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument UpdateExpiryDateForVulnerabilityException(string vulnExceptionID, string date)
		{
			string cmd = "<VulnerabilityExceptionUpdateExpiryDateRequest session-id=\"" + _session.SessionID + "\" exception-id=\"" + vulnExceptionID + "\" expiration-date=\"" + date + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument CreateMultiTenantUser(XmlNode userConfig)
		{
			string cmd = "<CreateMultiTenantUserRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + userConfig.OuterXml;
			cmd = cmd + "<CreateMultiTenantUserRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument GetMultiTenantUserListing()
		{
			string cmd = "<MultiTenantUserListingRequest session-id=\"" + _session.SessionID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument UpdateMultiTenantUser(XmlNode user)
		{
			string cmd = "<MultiTenantUserUpdateRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + user.OuterXml;
			cmd = cmd + "</MultiTenantUserUpdateRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument GetMultiTenantUserConfig(string userID)
		{
			string cmd = "<MultiTenantUserConfigRequest session-id=\"" + _session.SessionID + "\" user-id=\"" + userID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument DeleteMultiTenantUser(string userID)
		{
			string cmd = "<MultiTenantUserDeleteRequest session-id=\"" + _session.SessionID + "\" user-id=\"" + userID + "\"  />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument CreateSiloProfile(XmlNode siloConfig)
		{
			string cmd = "<SiloProfileCreateRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + siloConfig.OuterXml + "</SiloProfileCreateRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
		}
		
		public XmlDocument GetSiloProfileListing()
		{
			string cmd ="<SiloProfileListingRequest session-id=\"" + _session.SessionID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument UpdateSiloProfile(XmlNode siloProfile)
		{
			string cmd = "<SiloProfileUpdateRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + siloProfile.OuterXml;
			cmd = cmd + "</SiloProfileUpdateRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument GetSiloProfileConfig(string siloProfileID)
		{
			string cmd = "<SiloProfileConfigRequest session-id=\"" + _session.SessionID + "\" silo-profile-id=\"" + siloProfileID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument DeleteSiloProfile(string siloProfileID)
		{
			string cmd = "<SiloProfileDeletRequest session-id=\"" + _session.SessionID + "\" silo-profile-id=\"" + siloProfileID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument CreateSilo(XmlNode siloConfig)
		{
			string cmd = "<SiloCreateRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + siloConfig.OuterXml + "</SiloCreateRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument GetSiloListing()
		{
			string cmd = "<SiloListingRequest session-id=\"" + _session.SessionID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument GetSiloConfig(string siloID, string siloName)
		{
			string cmd = "<SiloConfigRequest session-id=\"" + _session.SessionID + "\" id=\"" + siloID + "\" name=\"" + CleanString(siloName) + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument UpdateSilo(XmlNode silo)
		{
			string cmd = "<SiloUpdateRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + silo.OuterXml;
			
			cmd = cmd + "</SiloUpdateRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument DeleteSilo(string siloID, string siloName)
		{
			string cmd = "<SiloDeleteRequest session-id=\"" + _session.SessionID + "\" silo-id=\"" + siloID + "\" silo-name=\"" + CleanString(siloName) + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument CreateRole(XmlNode role)
		{
			string cmd = "<RoleCreateRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + role.OuterXml;
			cmd = cmd + "</RoleCreateRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument GetRoleListing()
		{
			string cmd = "<RoleListingRequest session-id=\"" + _session.SessionID + "\" />";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument GetRoleDetails(string roleName)
		{
			string cmd = "<RoleDetailsRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + "<Role name=\"" + roleName + "\" />";
			cmd = cmd + "</RoleDetailsRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument UpdateRole(XmlNode role)
		{
			string cmd = "<RoleUpdateRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + role.OuterXml;
			
			cmd = cmd + "</RoleUpdateRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument DeleteRole(string roleName)
		{
			string cmd = "<RoleDeleteRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + "<Role name=\"" + roleName + "\" />";
			cmd = cmd + "</RoleDeleteRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument CreateEnginePool(XmlNode pool)
		{
			string cmd = "<EnginePoolCreateRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + pool.OuterXml + "</EnginePoolCreateRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument GetEnginePoolListing()
		{
			string cmd = "<EnginePoolListingRequest session-id=\"" + _session.SessionID + "\" >";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument GetEnginePoolDetails(string enginePoolName)
		{
			string cmd = "<EnginePoolDetailsRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + "<EnginePool name=\"" + CleanString(enginePoolName) + "\" /></EnginePoolDetailsRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument UpdateEnginePool(XmlNode pool)
		{
			string cmd = "<EnginePoolUpdateRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + pool.OuterXml;
			
			cmd = cmd + "</EnginePoolUpdateRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
			
		}
		
		public XmlDocument DeleteEnginePool(string enginePoolname)
		{
			string cmd = "<EnginePoolDeleteRequest session-id=\"" + _session.SessionID + "\" >";
			
			cmd = cmd + "<EnginePool name=\"" + CleanString(enginePoolname) + "\" /></EnginePoolDeleteRequest>";
			return _session.ExecuteCommand(cmd) as XmlDocument;
		}
		
		
		private string CleanString(string text)
		{
			return text.Replace(" & ", " &amp; ")
				.Replace("<", "&lt;")
				.Replace(">", "&gt;");
		}
		
	}
}

