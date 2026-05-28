Imports System
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Web.Configuration

Public Class _Default
    Inherits System.Web.UI.Page

    Private Const CONNECTION_STRING_KEY As String = "KYCConnString"

    ''' <summary>
    ''' Handles the Page Load event.
    ''' </summary>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Set the Date of Application to today's date
            Dim txtAppDate As TextBox = CType(FindControlRecursive(Me, "txtApplicationDate"), TextBox)
            If txtAppDate IsNot Nothing Then
                txtAppDate.Text = DateTime.Today.ToString("yyyy-MM-dd")
            End If
        End If
    End Sub

    ''' <summary>
    ''' Backend Form Submission Handler. Called on clicking the premium HTML submit button.
    ''' </summary>
    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            ' 1. Bootstraps and ensures database/table structures are fully configured
            EnsureDatabaseSchema()

            ' 2. Dynamically retrieve all frontend controls recursively to prevent VS conflicts
            Dim ddlAcType As DropDownList = CType(FindControlRecursive(Me, "ddlAccountType"), DropDownList)
            Dim ddlPrefBranch As DropDownList = CType(FindControlRecursive(Me, "ddlBranch"), DropDownList)
            Dim txtEmail As TextBox = CType(FindControlRecursive(Me, "txtEmail"), TextBox)
            Dim txtEmailOTP As TextBox = CType(FindControlRecursive(Me, "txtEmailOTP"), TextBox)
            Dim txtMobile As TextBox = CType(FindControlRecursive(Me, "txtMobileNumber"), TextBox)
            Dim txtAltMobile As TextBox = CType(FindControlRecursive(Me, "txtAlternateMobile"), TextBox)
            Dim txtAadhaarMobileOTP As TextBox = CType(FindControlRecursive(Me, "txtAadhaarMobileOTP"), TextBox)
            Dim txtAadhaarNum As TextBox = CType(FindControlRecursive(Me, "txtAadhaarNumber"), TextBox)
            Dim txtAadhaarOTP As TextBox = CType(FindControlRecursive(Me, "txtAadhaarOTP"), TextBox)
            Dim txtAadhaarName As TextBox = CType(FindControlRecursive(Me, "txtAadhaarName"), TextBox)
            Dim txtAadhaarDOB As TextBox = CType(FindControlRecursive(Me, "txtAadhaarDOB"), TextBox)
            Dim txtFullName As TextBox = CType(FindControlRecursive(Me, "txtFullName"), TextBox)
            Dim txtFatherName As TextBox = CType(FindControlRecursive(Me, "txtFatherName"), TextBox)
            Dim txtMotherName As TextBox = CType(FindControlRecursive(Me, "txtMotherName"), TextBox)
            Dim txtSpouse As TextBox = CType(FindControlRecursive(Me, "txtSpouseGuardian"), TextBox)
            Dim ddlMarital As DropDownList = CType(FindControlRecursive(Me, "ddlMaritalStatus"), DropDownList)
            Dim txtNat As TextBox = CType(FindControlRecursive(Me, "txtNationality"), TextBox)
            Dim txtRel As TextBox = CType(FindControlRecursive(Me, "txtReligion"), TextBox)
            Dim ddlResStatus As DropDownList = CType(FindControlRecursive(Me, "ddlResidentialStatus"), DropDownList)
            Dim txtBirthPlace As TextBox = CType(FindControlRecursive(Me, "txtPlaceOfBirth"), TextBox)
            Dim txtBirthCountry As TextBox = CType(FindControlRecursive(Me, "txtCountryOfBirth"), TextBox)
            Dim txtStreet As TextBox = CType(FindControlRecursive(Me, "txtStreet"), TextBox)
            Dim txtLocality As TextBox = CType(FindControlRecursive(Me, "txtLocality"), TextBox)
            Dim txtTown As TextBox = CType(FindControlRecursive(Me, "txtTown"), TextBox)
            Dim txtPO As TextBox = CType(FindControlRecursive(Me, "txtPostOffice"), TextBox)
            Dim txtCity As TextBox = CType(FindControlRecursive(Me, "txtCity"), TextBox)
            Dim ddlState As DropDownList = CType(FindControlRecursive(Me, "ddlState"), DropDownList)
            Dim txtCountry As TextBox = CType(FindControlRecursive(Me, "txtCountry"), TextBox)
            Dim txtPin As TextBox = CType(FindControlRecursive(Me, "txtPincode"), TextBox)
            Dim ddlAddrType As DropDownList = CType(FindControlRecursive(Me, "ddlAddressType"), DropDownList)
            Dim txtPermAddr As TextBox = CType(FindControlRecursive(Me, "txtPermanentAddress"), TextBox)
            Dim ddlOcc As DropDownList = CType(FindControlRecursive(Me, "ddlOccupation"), DropDownList)
            Dim txtEmpName As TextBox = CType(FindControlRecursive(Me, "txtEmployerName"), TextBox)
            Dim txtBusNature As TextBox = CType(FindControlRecursive(Me, "txtBusinessNature"), TextBox)
            Dim txtRole As TextBox = CType(FindControlRecursive(Me, "txtDesignation"), TextBox)
            Dim ddlIncome As DropDownList = CType(FindControlRecursive(Me, "ddlIncomeRange"), DropDownList)
            Dim txtPAN As TextBox = CType(FindControlRecursive(Me, "txtPANNumber"), TextBox)
            Dim txtPANName As TextBox = CType(FindControlRecursive(Me, "txtPANHolderName"), TextBox)
            Dim txtDLNum As TextBox = CType(FindControlRecursive(Me, "txtDLNumber"), TextBox)
            Dim txtDLDOB As TextBox = CType(FindControlRecursive(Me, "txtDLDOB"), TextBox)
            Dim txtDLName As TextBox = CType(FindControlRecursive(Me, "txtDLName"), TextBox)

            ' HTML Radio Toggles resolution
            Dim rdoInd As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoIndividual"), HtmlInputRadioButton)
            Dim rdoSameYes As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoSameYes"), HtmlInputRadioButton)
            
            ' Source of funds radios resolution
            Dim rdoFundsSalary As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoFundsSalary"), HtmlInputRadioButton)
            Dim rdoFundsBusiness As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoFundsBusiness"), HtmlInputRadioButton)
            Dim rdoFundsInvestments As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoFundsInvestments"), HtmlInputRadioButton)
            
            ' Document FileUpload resolution
            Dim fileAadhaar As FileUpload = CType(FindControlRecursive(Me, "fileAadhaar"), FileUpload)
            Dim filePAN As FileUpload = CType(FindControlRecursive(Me, "filePAN"), FileUpload)
            Dim filePassportDL As FileUpload = CType(FindControlRecursive(Me, "filePassportDL"), FileUpload)
            Dim fileAddressProof As FileUpload = CType(FindControlRecursive(Me, "fileAddressProof"), FileUpload)
            Dim fileSignature As FileUpload = CType(FindControlRecursive(Me, "fileSignature"), FileUpload)

            ' Determine active radio checked texts
            Dim custType As String = If(rdoInd IsNot Nothing AndAlso rdoInd.Checked, "Individual", "Non-Individual")
            Dim gender As String = GetSelectedGender()
            Dim isSameAddress As String = If(rdoSameYes IsNot Nothing AndAlso rdoSameYes.Checked, "Yes", "No")
            Dim fundsSource As String = "Others"
            If rdoFundsSalary IsNot Nothing AndAlso rdoFundsSalary.Checked Then
                fundsSource = "Salary"
            ElseIf rdoFundsBusiness IsNot Nothing AndAlso rdoFundsBusiness.Checked Then
                fundsSource = "Business"
            ElseIf rdoFundsInvestments IsNot Nothing AndAlso rdoFundsInvestments.Checked Then
                fundsSource = "Investments"
            End If

            ' 3. Server-Side Duplicate Check Validation (Aadhaar & PAN unique constraints)
            Dim cleanAadhaar As String = txtAadhaarNum.Text.Trim()
            Dim cleanPAN As String = txtPAN.Text.Trim().ToUpper()

            Dim duplicateMsg As String = CheckDuplicates(cleanAadhaar, cleanPAN)
            If Not String.IsNullOrEmpty(duplicateMsg) Then
                InjectServerToast("Profile Conflict", duplicateMsg, "danger")
                Return
            End If

            ' 4. Server-Side File Upload Streams Validation (Format & Size <= 2MB)
            If Not ValidateAndSaveFiles(fileAadhaar, filePAN, filePassportDL, fileAddressProof, fileSignature, _
                                         outAadhaarPath, outPANPath, outDLPath, outAddrPath, outSignPath, errorMsg) Then
                InjectServerToast("Upload Failure", errorMsg, "danger")
                Return
            End If

            ' 5. Perform Secure Database Insertion using Parameterized Query to prevent SQL injection
            Dim connString As String = WebConfigurationManager.ConnectionStrings(CONNECTION_STRING_KEY).ConnectionString
            Dim insertQuery As String = "INSERT INTO KYCDetails (" & _
                "AccountType, CustomerType, PreferredBranch, ApplicationDate, Email, EmailOTP, MobileNumber, " & _
                "AlternateMobileNumber, AadhaarMobileOTP, AadhaarNumber, AadhaarOTP, AadhaarName, AadhaarDOB, Gender, " & _
                "FullLegalName, FatherName, MotherName, SpouseGuardianName, MaritalStatus, Nationality, Religion, " & _
                "ResidentialStatus, PlaceOfBirth, CountryOfBirth, StreetHouseLandmark, AreaLocality, LocationVillageTown, " & _
                "PostOffice, CityDistrict, State, Country, Pincode, TypeOfAddress, IsPermanentAddressSame, PermanentAddress, " & _
                "OccupationType, EmployerName, NatureOfBusiness, Designation, AnnualIncomeRange, SourceOfFunds, " & _
                "PANNumber, PANHolderName, DrivingLicenceNumber, DrivingLicenceDOB, DrivingLicenceName, " & _
                "AadhaarCardPath, PANCardPath, PassportDLPath, AddressProofPath, SignatureScanPath" & _
                ") VALUES (" & _
                "@AcType, @CustType, @Branch, @AppDate, @Email, @EmailOTP, @Mobile, @AltMobile, @AadhaarMobileOTP, " & _
                "@AadhaarNum, @AadhaarOTP, @AadhaarName, @AadhaarDOB, @Gender, @FullName, @FatherName, @MotherName, " & _
                "@Spouse, @Marital, @Nat, @Rel, @ResStatus, @PlaceOfBirth, @CountryOfBirth, @Street, @Locality, @Town, " & _
                "@PO, @City, @State, @Country, @Pin, @AddrType, @IsSame, @PermAddr, @Occ, @EmpName, @BusNature, @Role, " & _
                "@Income, @Funds, @PAN, @PANName, @DLNum, @DLDOB, @DLName, @AadhaarPath, @PANPath, @DLPath, @AddrPath, @SignPath)"

            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand(insertQuery, conn)
                    ' Bind all parameters safely
                    cmd.Parameters.AddWithValue("@AcType", ddlAcType.SelectedValue)
                    cmd.Parameters.AddWithValue("@CustType", custType)
                    cmd.Parameters.AddWithValue("@Branch", ddlPrefBranch.SelectedValue)
                    cmd.Parameters.AddWithValue("@AppDate", DateTime.Today)
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim())
                    cmd.Parameters.AddWithValue("@EmailOTP", txtEmailOTP.Text.Trim())
                    cmd.Parameters.AddWithValue("@Mobile", txtMobile.Text.Trim())
                    cmd.Parameters.AddWithValue("@AltMobile", If(String.IsNullOrEmpty(txtAltMobile.Text.Trim()), DBNull.Value, txtAltMobile.Text.Trim()))
                    cmd.Parameters.AddWithValue("@AadhaarMobileOTP", txtAadhaarMobileOTP.Text.Trim())
                    cmd.Parameters.AddWithValue("@AadhaarNum", cleanAadhaar)
                    cmd.Parameters.AddWithValue("@AadhaarOTP", txtAadhaarOTP.Text.Trim())
                    cmd.Parameters.AddWithValue("@AadhaarName", txtAadhaarName.Text.Trim())
                    cmd.Parameters.AddWithValue("@AadhaarDOB", DateTime.Parse(txtAadhaarDOB.Text.Trim()))
                    cmd.Parameters.AddWithValue("@Gender", gender)
                    cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim())
                    cmd.Parameters.AddWithValue("@FatherName", txtFatherName.Text.Trim())
                    cmd.Parameters.AddWithValue("@MotherName", txtMotherName.Text.Trim())
                    cmd.Parameters.AddWithValue("@Spouse", txtSpouse.Text.Trim())
                    cmd.Parameters.AddWithValue("@Marital", ddlMarital.SelectedValue)
                    cmd.Parameters.AddWithValue("@Nat", txtNat.Text.Trim())
                    cmd.Parameters.AddWithValue("@Rel", If(String.IsNullOrEmpty(txtRel.Text.Trim()), DBNull.Value, txtRel.Text.Trim()))
                    cmd.Parameters.AddWithValue("@ResStatus", ddlResStatus.SelectedValue)
                    cmd.Parameters.AddWithValue("@PlaceOfBirth", If(String.IsNullOrEmpty(txtBirthPlace.Text.Trim()), DBNull.Value, txtBirthPlace.Text.Trim()))
                    cmd.Parameters.AddWithValue("@CountryOfBirth", If(String.IsNullOrEmpty(txtBirthCountry.Text.Trim()), DBNull.Value, txtBirthCountry.Text.Trim()))
                    cmd.Parameters.AddWithValue("@Street", txtStreet.Text.Trim())
                    cmd.Parameters.AddWithValue("@Locality", txtLocality.Text.Trim())
                    cmd.Parameters.AddWithValue("@Town", txtTown.Text.Trim())
                    cmd.Parameters.AddWithValue("@PO", txtPO.Text.Trim())
                    cmd.Parameters.AddWithValue("@City", txtCity.Text.Trim())
                    cmd.Parameters.AddWithValue("@State", ddlState.SelectedValue)
                    cmd.Parameters.AddWithValue("@Country", txtCountry.Text.Trim())
                    cmd.Parameters.AddWithValue("@Pin", txtPin.Text.Trim())
                    cmd.Parameters.AddWithValue("@AddrType", ddlAddrType.SelectedValue)
                    cmd.Parameters.AddWithValue("@IsSame", isSameAddress)
                    cmd.Parameters.AddWithValue("@PermAddr", If(isSameAddress = "Yes", DBNull.Value, txtPermAddr.Text.Trim()))
                    cmd.Parameters.AddWithValue("@Occ", ddlOcc.SelectedValue)
                    cmd.Parameters.AddWithValue("@EmpName", If(String.IsNullOrEmpty(txtEmpName.Text.Trim()), DBNull.Value, txtEmpName.Text.Trim()))
                    cmd.Parameters.AddWithValue("@BusNature", If(String.IsNullOrEmpty(txtBusNature.Text.Trim()), DBNull.Value, txtBusNature.Text.Trim()))
                    cmd.Parameters.AddWithValue("@Role", If(String.IsNullOrEmpty(txtRole.Text.Trim()), DBNull.Value, txtRole.Text.Trim()))
                    cmd.Parameters.AddWithValue("@Income", ddlIncome.SelectedValue)
                    cmd.Parameters.AddWithValue("@Funds", fundsSource)
                    cmd.Parameters.AddWithValue("@PAN", cleanPAN)
                    cmd.Parameters.AddWithValue("@PANName", txtPANName.Text.Trim())
                    cmd.Parameters.AddWithValue("@DLNum", If(String.IsNullOrEmpty(txtDLNum.Text.Trim()), DBNull.Value, txtDLNum.Text.Trim().ToUpper()))
                    cmd.Parameters.AddWithValue("@DLDOB", If(String.IsNullOrEmpty(txtDLDOB.Text.Trim()), DBNull.Value, DateTime.Parse(txtDLDOB.Text.Trim())))
                    cmd.Parameters.AddWithValue("@DLName", If(String.IsNullOrEmpty(txtDLName.Text.Trim()), DBNull.Value, txtDLName.Text.Trim()))
                    
                    ' Paths stored securely
                    cmd.Parameters.AddWithValue("@AadhaarPath", outAadhaarPath)
                    cmd.Parameters.AddWithValue("@PANPath", outPANPath)
                    cmd.Parameters.AddWithValue("@DLPath", If(String.IsNullOrEmpty(outDLPath), DBNull.Value, outDLPath))
                    cmd.Parameters.AddWithValue("@AddrPath", If(String.IsNullOrEmpty(outAddrPath), DBNull.Value, outAddrPath))
                    cmd.Parameters.AddWithValue("@SignPath", outSignPath)

                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' 6. Inject beautiful JavaScript success notification to wipe local progress cache and toast success
            InjectServerToast("KYC Submission Success!", "Your digital KYC profile has been verified and registered securely in the core banking database.", "success", clearAutosave:=True)

        Catch ex As Exception
            InjectServerToast("Database Error", "Failed to register KYC details: " & ex.Message.Replace("'", "\'"), "danger")
        End Try
    End Sub

    ' Temporary Out Strings for File Upload saving refs
    Private outAadhaarPath As String = ""
    Private outPANPath As String = ""
    Private outDLPath As String = ""
    Private outAddrPath As String = ""
    Private outSignPath As String = ""
    Private errorMsg As String = ""

    ''' <summary>
    ''' Resolves the checked gender description.
    ''' </summary>
    Private Function GetSelectedGender() As String
        Dim rdoM As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoMale"), HtmlInputRadioButton)
        Dim rdoF As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoFemale"), HtmlInputRadioButton)
        If rdoM IsNot Nothing AndAlso rdoM.Checked Then Return "Male"
        If rdoF IsNot Nothing AndAlso rdoF.Checked Then Return "Female"
        Return "Other"
    End Function

    ''' <summary>
    ''' Validates file uploads (extension and size less than or equal to 2MB) and saves them securely inside /Uploads folder.
    ''' </summary>
    Private Function ValidateAndSaveFiles(ByVal fAadhaar As FileUpload, ByVal fPAN As FileUpload, ByVal fDL As FileUpload, ByVal fAddr As FileUpload, ByVal fSign As FileUpload, ByRef pathAadhaar As String, ByRef pathPAN As String, ByRef pathDL As String, ByRef pathAddr As String, ByRef pathSign As String, ByRef errOut As String) As Boolean
        Try
            ' Create Uploads directory in root if it does not exist
            Dim uploadFolder As String = Server.MapPath("~/Uploads")
            If Not Directory.Exists(uploadFolder) Then
                Directory.CreateDirectory(uploadFolder)
            End If

            ' Mandatory uploads check
            If fAadhaar Is Nothing OrElse Not fAadhaar.HasFile Then
                errOut = "Aadhaar Card document upload is required."
                Return False
            End If
            If fPAN Is Nothing OrElse Not fPAN.HasFile Then
                errOut = "PAN Card document upload is required."
                Return False
            End If
            If fSign Is Nothing OrElse Not fSign.HasFile Then
                errOut = "Signature Scan upload is required."
                Return False
            End If

            ' Process files
            If Not SaveSingleFile(fAadhaar, "Aadhaar", uploadFolder, pathAadhaar, errOut) Then Return False
            If Not SaveSingleFile(fPAN, "PAN", uploadFolder, pathPAN, errOut) Then Return False
            If fDL IsNot Nothing AndAlso fDL.HasFile Then
                If Not SaveSingleFile(fDL, "PassportDL", uploadFolder, pathDL, errOut) Then Return False
            End If
            If fAddr IsNot Nothing AndAlso fAddr.HasFile Then
                If Not SaveSingleFile(fAddr, "AddressProof", uploadFolder, pathAddr, errOut) Then Return False
            End If
            If Not SaveSingleFile(fSign, "Signature", uploadFolder, pathSign, errOut) Then Return False

            Return True
        Catch ex As Exception
            errOut = "Error processing upload files: " & ex.Message
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Saves a single uploaded file securely.
    ''' </summary>
    Private Function SaveSingleFile(ByVal fileCtrl As FileUpload, ByVal prefix As String, ByVal folder As String, ByRef outPath As String, ByRef errOut As String) As Boolean
        Dim maxFileSize As Integer = 2 * 1024 * 1024 ' 2MB
        If fileCtrl.PostedFile.ContentLength > maxFileSize Then
            errOut = String.Format("{0} exceeds the maximum file size limit of 2MB.", fileCtrl.FileName)
            Return False
        End If

        Dim ext As String = Path.GetExtension(fileCtrl.FileName).ToLower()
        If ext <> ".pdf" AndAlso ext <> ".jpg" AndAlso ext <> ".jpeg" Then
            errOut = String.Format("File type {0} not allowed. Only PDF and JPG/JPEG files are accepted.", fileCtrl.FileName)
            Return False
        End If

        ' Unique secure filename using ticks timestamp and prefix
        Dim fileName As String = String.Format("{0}_{1}{2}", prefix, DateTime.UtcNow.Ticks, ext)
        Dim fullPath As String = Path.Combine(folder, fileName)
        fileCtrl.SaveAs(fullPath)
        
        outPath = "~/Uploads/" & fileName
        Return True
    End Function

    ''' <summary>
    ''' Performs server-side duplicate checking on Aadhaar and PAN fields.
    ''' </summary>
    Private Function CheckDuplicates(ByVal aadhaar As String, ByVal pan As String) As String
        Dim connString As String = WebConfigurationManager.ConnectionStrings(CONNECTION_STRING_KEY).ConnectionString
        Dim query As String = "SELECT " & _
            "SUM(CASE WHEN AadhaarNumber = @Aadhaar THEN 1 ELSE 0 END) As AadhaarCount, " & _
            "SUM(CASE WHEN PANNumber = @PAN THEN 1 ELSE 0 END) As PANCount " & _
            "FROM KYCDetails"

        Try
            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Aadhaar", aadhaar)
                    cmd.Parameters.AddWithValue("@PAN", pan)

                    conn.Open()
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim aadhaarCount As Integer = If(reader("AadhaarCount") Is DBNull.Value, 0, Convert.ToInt32(reader("AadhaarCount")))
                            Dim panCount As Integer = If(reader("PANCount") Is DBNull.Value, 0, Convert.ToInt32(reader("PANCount")))

                            If aadhaarCount > 0 AndAlso panCount > 0 Then
                                Return "This Aadhaar Number and PAN Number are already registered to active profiles."
                            End If
                            If aadhaarCount > 0 Then
                                Return "A KYC profile with this Aadhaar Number already exists."
                            End If
                            If panCount > 0 Then
                                Return "A KYC profile with this PAN Number already exists."
                            End If
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Database might not exist yet during initialization checks, skip
        End Try
        Return ""
    End Function

    ''' <summary>
    ''' Bootstraps the local LocalDB configuration and creates DB/Table structures automatically if missing.
    ''' </summary>
    Private Sub EnsureDatabaseSchema()
        ' Connection string to master DB to verify database presence
        Dim masterConnStr As String = "Server=(localdb)\MSSQLLocalDB;Database=master;Integrated Security=True;"
        Dim mainConnStr As String = WebConfigurationManager.ConnectionStrings(CONNECTION_STRING_KEY).ConnectionString

        ' Create DB if missing
        Try
            Using conn As New SqlConnection(masterConnStr)
                Dim dbQuery As String = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'KYCDB') CREATE DATABASE KYCDB;"
                Using cmd As New SqlCommand(dbQuery, conn)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception("Failed to ensure database creation on LocalDB: " & ex.Message, ex)
        End Try

        ' Create Table if missing
        Try
            Using conn As New SqlConnection(mainConnStr)
                Dim tblQuery As String = "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KYCDetails]') AND type in (N'U')) " & _
                    "CREATE TABLE [dbo].[KYCDetails] (" & _
                    "[Id] INT IDENTITY(1,1) PRIMARY KEY, " & _
                    "[AccountType] NVARCHAR(50) NOT NULL, " & _
                    "[CustomerType] NVARCHAR(50) NOT NULL, " & _
                    "[PreferredBranch] NVARCHAR(100) NOT NULL, " & _
                    "[ApplicationDate] DATE NOT NULL, " & _
                    "[Email] NVARCHAR(150) NOT NULL, " & _
                    "[EmailOTP] NVARCHAR(10) NOT NULL, " & _
                    "[MobileNumber] NVARCHAR(15) NOT NULL, " & _
                    "[AlternateMobileNumber] NVARCHAR(15) NULL, " & _
                    "[AadhaarMobileOTP] NVARCHAR(10) NOT NULL, " & _
                    "[AadhaarNumber] NVARCHAR(20) NOT NULL CONSTRAINT UQ_Aadhaar UNIQUE, " & _
                    "[AadhaarOTP] NVARCHAR(10) NOT NULL, " & _
                    "[AadhaarName] NVARCHAR(100) NOT NULL, " & _
                    "[AadhaarDOB] DATE NOT NULL, " & _
                    "[Gender] NVARCHAR(20) NOT NULL, " & _
                    "[FullLegalName] NVARCHAR(100) NOT NULL, " & _
                    "[FatherName] NVARCHAR(100) NOT NULL, " & _
                    "[MotherName] NVARCHAR(100) NOT NULL, " & _
                    "[SpouseGuardianName] NVARCHAR(100) NOT NULL, " & _
                    "[MaritalStatus] NVARCHAR(50) NOT NULL, " & _
                    "[Nationality] NVARCHAR(50) NOT NULL, " & _
                    "[Religion] NVARCHAR(50) NULL, " & _
                    "[ResidentialStatus] NVARCHAR(50) NOT NULL, " & _
                    "[PlaceOfBirth] NVARCHAR(100) NULL, " & _
                    "[CountryOfBirth] NVARCHAR(100) NULL, " & _
                    "[StreetHouseLandmark] NVARCHAR(250) NOT NULL, " & _
                    "[AreaLocality] NVARCHAR(150) NOT NULL, " & _
                    "[LocationVillageTown] NVARCHAR(150) NOT NULL, " & _
                    "[PostOffice] NVARCHAR(100) NOT NULL, " & _
                    "[CityDistrict] NVARCHAR(100) NOT NULL, " & _
                    "[State] NVARCHAR(100) NOT NULL, " & _
                    "[Country] NVARCHAR(100) NOT NULL, " & _
                    "[Pincode] NVARCHAR(10) NOT NULL, " & _
                    "[TypeOfAddress] NVARCHAR(50) NOT NULL, " & _
                    "[IsPermanentAddressSame] NVARCHAR(5) NOT NULL, " & _
                    "[PermanentAddress] NVARCHAR(500) NULL, " & _
                    "[OccupationType] NVARCHAR(50) NOT NULL, " & _
                    "[EmployerName] NVARCHAR(150) NULL, " & _
                    "[NatureOfBusiness] NVARCHAR(150) NULL, " & _
                    "[Designation] NVARCHAR(100) NULL, " & _
                    "[AnnualIncomeRange] NVARCHAR(100) NOT NULL, " & _
                    "[SourceOfFunds] NVARCHAR(100) NOT NULL, " & _
                    "[PANNumber] NVARCHAR(20) NOT NULL CONSTRAINT UQ_PAN UNIQUE, " & _
                    "[PANHolderName] NVARCHAR(100) NOT NULL, " & _
                    "[DrivingLicenceNumber] NVARCHAR(30) NULL, " & _
                    "[DrivingLicenceDOB] DATE NULL, " & _
                    "[DrivingLicenceName] NVARCHAR(100) NULL, " & _
                    "[AadhaarCardPath] NVARCHAR(500) NOT NULL, " & _
                    "[PANCardPath] NVARCHAR(500) NOT NULL, " & _
                    "[PassportDLPath] NVARCHAR(500) NULL, " & _
                    "[AddressProofPath] NVARCHAR(500) NULL, " & _
                    "[SignatureScanPath] NVARCHAR(500) NOT NULL, " & _
                    "[CreatedAt] DATETIME DEFAULT GETDATE()" & _
                    ");"
                Using cmd As New SqlCommand(tblQuery, conn)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception("Failed to bootstrap table structures on KYCDB: " & ex.Message, ex)
        End Try
    End Sub

    ''' <summary>
    ''' Helper method to dynamically search for a server control recursively.
    ''' </summary>
    Private Function FindControlRecursive(ByVal root As Control, ByVal id As String) As Control
        If root Is Nothing Then Return Nothing
        If root.ID = id Then Return root
        For Each child As Control In root.Controls
            Dim found As Control = FindControlRecursive(child, id)
            If found IsNot Nothing Then Return found
        Next
        Return Nothing
    End Function

    ''' <summary>
    ''' Dynamically injects elegant sliding toast notifications into the loaded webpage response stream.
    ''' </summary>
    Private Sub InjectServerToast(ByVal title As String, ByVal message As String, ByVal type As String, Optional ByVal clearAutosave As Boolean = False)
        Dim litServerToasts As Literal = CType(FindControlRecursive(Me, "litServerToasts"), Literal)
        If litServerToasts IsNot Nothing Then
            Dim clearScript As String = ""
            If clearAutosave Then
                clearScript = "localStorage.removeItem('kyc_form_progress');"
            End If
            
            Dim script As String = String.Format(
                "<script type='text/javascript'>" & vbCrLf &
                "    window.addEventListener('DOMContentLoaded', () => {{" & vbCrLf &
                "        {0}" & vbCrLf &
                "        showToast('{1}', '{2}', '{3}');" & vbCrLf &
                "    }});" & vbCrLf &
                "</script>",
                clearScript,
                title.Replace("'", "\'"),
                message.Replace("'", "\'"),
                type
            )
            litServerToasts.Text = script
        End If
    End Sub
End Class
