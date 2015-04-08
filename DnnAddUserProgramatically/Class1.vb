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

    Private Shared Function GetUserInfo() As UserInfo
        Dim a As New UserInfo
        a.FirstName = FirstName
        a.LastName = LastName
        a.PortalID = PortalID
        a.Email = EMail
        a.Username = UserName
        a.DisplayName = DisplayName
        a.Membership.Password = UserController.GeneratePassword(12).ToString
        a.IsSuperUser = False
        Return a
    End Function

    Public Shared Function CreateUser() As UserCreateStatus
        Dim createStatus As UserCreateStatus = UserCreateStatus.AddUser
        Dim user As UserInfo = GetUserInfo()
        'Create the User
        createStatus = memberProvider.CreateUser(user)
        If createStatus = UserCreateStatus.Success Then
            'Dim objEventLog As New Services.Log.EventLog.EventLogController
            'objEventLog.AddLog(objUser, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.USER_CREATED)
            DataCache.ClearPortalCache(user.PortalID, False)
            If Not user.IsSuperUser Then
                Dim objRoles As New RoleController
                Dim objRole As RoleInfo
                ' autoassign user to portal roles
                Dim arrRoles As ArrayList = objRoles.GetPortalRoles(user.PortalID)
                Dim i As Integer
                For i = 0 To arrRoles.Count - 1
                    objRole = CType(arrRoles(i), RoleInfo)
                    If objRole.AutoAssignment = True Then
                        objRoles.AddUserRole(user.PortalID, user.UserID, objRole.RoleID, Null.NullDate, Null.NullDate)
                    End If
                Next
                objRoles.AddUserRole(user.PortalID, user.UserID, 5, Null.NullDate, Null.NullDate)
            End If
        End If
        Return createStatus.ToString
    End Function

End Class