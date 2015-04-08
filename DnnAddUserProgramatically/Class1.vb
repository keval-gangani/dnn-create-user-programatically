Imports Microsoft.VisualBasic
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Common.Utilities

Public Class Class1

    Shared _firstName As String = ""
    Shared _lastName As String = ""
    Shared _username As String = ""
    Shared _displayName As String = ""
    Shared _email As String = ""
    Shared _portalID As Integer = -1
    Shared _password As String = ""

#Region "Properties"

    Public Shared Property FirstName As String
        Get
            Return _firstName
        End Get
        Set(ByVal value As String)
            _firstName = value
        End Set
    End Property

    Public Shared Property LastName As String
        Get
            Return _lastName
        End Get
        Set(ByVal value As String)
            _lastName = value
        End Set
    End Property

    Public Shared Property UserName As String
        Get
            Return _username
        End Get
        Set(ByVal value As String)
            _username = value
        End Set
    End Property

    Public Shared Property DisplayName As String
        Get
            If _displayName = "" Then
                _displayName = FirstName & " " & LastName
            End If
            Return _displayName
        End Get
        Set(ByVal value As String)
            _displayName = value
        End Set
    End Property

    Public Shared Property EMail As String
        Get
            Return _email
        End Get
        Set(ByVal value As String)
            _email = value
        End Set
    End Property

    Public Shared Property Password As String
        Get
            Return _password
        End Get
        Set(ByVal value As String)
            _password = value
        End Set
    End Property

    Public Shared Property PortalID As Integer
        Get
            If _portalID = -1 Then
                _portalID = DotNetNuke.Common.GetPortalSettings.PortalId
            End If
            Return _portalID
        End Get
        Set(ByVal value As Integer)
            _portalID = value
        End Set
    End Property

#End Region

#Region "Private Shared Members"

    Private Shared memberProvider As DotNetNuke.Security.Membership.MembershipProvider = DotNetNuke.Security.Membership.MembershipProvider.Instance()

#End Region

    Private Shared Function GetUserInfo(ByVal fiPortalId As Integer) As UserInfo
        Dim a As New UserInfo
        a.FirstName = FirstName
        a.LastName = LastName
        a.PortalID = fiPortalId
        a.Email = EMail
        a.Username = UserName
        a.DisplayName = DisplayName


        Dim objMembership As UserMembership = New UserMembership
        objMembership.Approved = True
        objMembership.CreatedDate = DateTime.Now
        objMembership.Email = EMail
        objMembership.Username = UserName
        objMembership.Password = Password

        a.Membership = objMembership
        a.IsSuperUser = False
        Return a
    End Function

    Public Shared Function CreateUser(ByVal fiPortalId As Integer) As UserCreateStatus
        Dim createStatus As UserCreateStatus = UserCreateStatus.AddUser
        Dim user As UserInfo = GetUserInfo(fiPortalId)
        'Create the User

        createStatus = memberProvider.CreateUser(user)
        If createStatus = UserCreateStatus.Success Then
            'Dim objEventLog As New Services.Log.EventLog.EventLogController
            'objEventLog.AddLog(objUser, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_CREATED)
            DataCache.ClearPortalCache(user.PortalID, False)
            addRoleToUser(user, "Google User", DateTime.Now.AddYears(25))
            'If Not user.IsSuperUser Then
            '    Dim objRoles As New RoleController
            '    Dim objRole As RoleInfo
            '    ' autoassign user to portal roles
            '    Dim arrRoles As ArrayList = objRoles.GetPortalRoles(user.PortalID)
            '    Dim i As Integer
            '    For i = 0 To arrRoles.Count - 1
            '        objRole = CType(arrRoles(i), RoleInfo)
            '        If objRole.AutoAssignment = True Then
            '            objRoles.AddUserRole(user.PortalID, user.UserID, objRole.RoleID, Null.NullDate, Null.NullDate)
            '        End If
            '    Next
            '    objRoles.AddUserRole(user.PortalID, user.UserID, 5, Null.NullDate, Null.NullDate)
            'End If
        End If
        Return createStatus
    End Function

    Public Shared Function addRoleToUser(ByRef user As UserInfo, ByVal roleName As String, ByRef expiry As DateTime) As Boolean
        Dim rc As Boolean = False
        Dim roleCtl As RoleController = New RoleController



        Dim newRole As RoleInfo = roleCtl.GetRoleByName(user.PortalID, roleName)

        If newRole IsNot Nothing And user IsNot Nothing Then
            roleCtl.AddUserRole(user.PortalID, user.UserID, newRole.RoleID, DateTime.MinValue, expiry)
            user = UserController.GetUserById(user.PortalID, user.UserID)
            rc = user.IsInRole(roleName)
            'ElseIf newRole Is Nothing And user IsNot Nothing Then
            '    Dim loRole As RoleInfo = New RoleInfo()
            '    loRole.IsPublic = True
            '    loRole.PortalID = user.PortalID
            '    loRole.RoleName = "Google User"
            '    loRole.Status = RoleStatus.Approved

            '    Dim roleid As Integer = roleCtl.AddRole(loRole)

            '    roleCtl.AddUserRole(user.PortalID, user.UserID, roleid, DateTime.MinValue, expiry)
            '    user = UserController.GetUserById(user.PortalID, user.UserID)
            '    rc = user.IsInRole(loRole.RoleName)

        End If

        Return rc
    End Function

End Class