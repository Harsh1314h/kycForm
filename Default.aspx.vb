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

            ' Check if we redirected after a successful KYC submission (PRG Pattern)
            If Request.QueryString("success") = "1" Then
                InjectServerToast("KYC Submission Success!", "Your digital KYC profile has been verified and registered securely in the core banking database.", "success", clearAutosave:=True)
            End If

            ' Handle Edit Mode routing
            Dim editIdStr As String = Request.QueryString("edit")
            If Not String.IsNullOrEmpty(editIdStr) Then
                Dim editId As Integer = 0
                If Integer.TryParse(editIdStr, editId) Then
                    LoadKYCRecordForEdit(editId)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Loads an existing KYC profile from the SQL Server database and populates all form fields.
    ''' </summary>
    Private Sub LoadKYCRecordForEdit(ByVal editId As Integer)
        Dim connString As String = WebConfigurationManager.ConnectionStrings(CONNECTION_STRING_KEY).ConnectionString
        Dim selectQuery As String = "SELECT * FROM KYCDetails WHERE Id = @Id"

        Try
            ' Ensure schema is fully bootstrapped
            EnsureDatabaseSchema()

            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand("sp_GetKYCRecordById", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@Id", editId)
                    conn.Open()
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            ' Populate state tracking hidden fields
                            Dim hdnEditId As HiddenField = CType(FindControlRecursive(Me, "hdnEditId"), HiddenField)
                            Dim hdnAadhaarPath As HiddenField = CType(FindControlRecursive(Me, "hdnAadhaarPath"), HiddenField)
                            Dim hdnPANPath As HiddenField = CType(FindControlRecursive(Me, "hdnPANPath"), HiddenField)
                            Dim hdnDLPath As HiddenField = CType(FindControlRecursive(Me, "hdnDLPath"), HiddenField)
                            Dim hdnAddrPath As HiddenField = CType(FindControlRecursive(Me, "hdnAddrPath"), HiddenField)
                            Dim hdnSignPath As HiddenField = CType(FindControlRecursive(Me, "hdnSignPath"), HiddenField)

                            If hdnEditId IsNot Nothing Then hdnEditId.Value = editId.ToString()
                            If hdnAadhaarPath IsNot Nothing Then hdnAadhaarPath.Value = reader("AadhaarCardPath").ToString()
                            If hdnPANPath IsNot Nothing Then hdnPANPath.Value = reader("PANCardPath").ToString()
                            If hdnDLPath IsNot Nothing Then hdnDLPath.Value = If(reader("PassportDLPath") Is DBNull.Value, "", reader("PassportDLPath").ToString())
                            If hdnAddrPath IsNot Nothing Then hdnAddrPath.Value = If(reader("AddressProofPath") Is DBNull.Value, "", reader("AddressProofPath").ToString())
                            If hdnSignPath IsNot Nothing Then hdnSignPath.Value = reader("SignatureScanPath").ToString()

                            ' Show Edit Mode visual banner
                            Dim pnlEditMode As PlaceHolder = CType(FindControlRecursive(Me, "pnlEditMode"), PlaceHolder)
                            Dim lblEditId As Label = CType(FindControlRecursive(Me, "lblEditId"), Label)
                            If pnlEditMode IsNot Nothing Then pnlEditMode.Visible = True
                            If lblEditId IsNot Nothing Then lblEditId.Text = editId.ToString()

                            ' Bind dropdown fields
                            Dim ddlAcType As DropDownList = CType(FindControlRecursive(Me, "ddlAccountType"), DropDownList)
                            If ddlAcType IsNot Nothing Then ddlAcType.SelectedValue = reader("AccountType").ToString()

                            Dim ddlPrefBranch As DropDownList = CType(FindControlRecursive(Me, "ddlBranch"), DropDownList)
                            If ddlPrefBranch IsNot Nothing Then ddlPrefBranch.SelectedValue = reader("PreferredBranch").ToString()

                            Dim txtAppDate As TextBox = CType(FindControlRecursive(Me, "txtApplicationDate"), TextBox)
                            If txtAppDate IsNot Nothing Then
                                txtAppDate.Text = Convert.ToDateTime(reader("ApplicationDate")).ToString("yyyy-MM-dd")
                            End If

                            ' Bind Customer Type radios
                            Dim rdoInd As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoIndividual"), HtmlInputRadioButton)
                            Dim rdoNonInd As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoNonIndividual"), HtmlInputRadioButton)
                            If rdoInd IsNot Nothing AndAlso rdoNonInd IsNot Nothing Then
                                If reader("CustomerType").ToString() = "Individual" Then
                                    rdoInd.Checked = True
                                    rdoNonInd.Checked = False
                                Else
                                    rdoInd.Checked = False
                                    rdoNonInd.Checked = True
                                End If
                            End If

                            ' Contact details
                            CType(FindControlRecursive(Me, "txtEmail"), TextBox).Text = reader("Email").ToString()
                            CType(FindControlRecursive(Me, "txtEmailOTP"), TextBox).Text = reader("EmailOTP").ToString()
                            CType(FindControlRecursive(Me, "txtMobileNumber"), TextBox).Text = reader("MobileNumber").ToString()
                            CType(FindControlRecursive(Me, "txtAadhaarMobileOTP"), TextBox).Text = reader("AadhaarMobileOTP").ToString()
                            CType(FindControlRecursive(Me, "txtAlternateMobile"), TextBox).Text = If(reader("AlternateMobileNumber") Is DBNull.Value, "", reader("AlternateMobileNumber").ToString())

                            ' Aadhaar fields
                            CType(FindControlRecursive(Me, "txtAadhaarNumber"), TextBox).Text = reader("AadhaarNumber").ToString()
                            CType(FindControlRecursive(Me, "txtAadhaarOTP"), TextBox).Text = reader("AadhaarOTP").ToString()
                            CType(FindControlRecursive(Me, "txtAadhaarName"), TextBox).Text = reader("AadhaarName").ToString()
                            CType(FindControlRecursive(Me, "txtAadhaarDOB"), TextBox).Text = Convert.ToDateTime(reader("AadhaarDOB")).ToString("yyyy-MM-dd")

                            ' Gender radios
                            Dim rdoM As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoMale"), HtmlInputRadioButton)
                            Dim rdoF As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoFemale"), HtmlInputRadioButton)
                            Dim rdoO As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoOther"), HtmlInputRadioButton)
                            If rdoM IsNot Nothing AndAlso rdoF IsNot Nothing AndAlso rdoO IsNot Nothing Then
                                rdoM.Checked = False : rdoF.Checked = False : rdoO.Checked = False
                                If reader("Gender").ToString() = "Male" Then
                                    rdoM.Checked = True
                                ElseIf reader("Gender").ToString() = "Female" Then
                                    rdoF.Checked = True
                                Else
                                    rdoO.Checked = True
                                End If
                            End If

                            ' Personal Information
                            CType(FindControlRecursive(Me, "txtFullName"), TextBox).Text = reader("FullLegalName").ToString()
                            CType(FindControlRecursive(Me, "txtFatherName"), TextBox).Text = reader("FatherName").ToString()
                            CType(FindControlRecursive(Me, "txtMotherName"), TextBox).Text = reader("MotherName").ToString()
                            CType(FindControlRecursive(Me, "txtSpouseGuardian"), TextBox).Text = reader("SpouseGuardianName").ToString()
                            CType(FindControlRecursive(Me, "ddlMaritalStatus"), DropDownList).SelectedValue = reader("MaritalStatus").ToString()
                            CType(FindControlRecursive(Me, "txtNationality"), TextBox).Text = reader("Nationality").ToString()
                            CType(FindControlRecursive(Me, "txtReligion"), TextBox).Text = If(reader("Religion") Is DBNull.Value, "", reader("Religion").ToString())
                            CType(FindControlRecursive(Me, "ddlResidentialStatus"), DropDownList).SelectedValue = reader("ResidentialStatus").ToString()
                            CType(FindControlRecursive(Me, "txtPlaceOfBirth"), TextBox).Text = If(reader("PlaceOfBirth") Is DBNull.Value, "", reader("PlaceOfBirth").ToString())
                            CType(FindControlRecursive(Me, "txtCountryOfBirth"), TextBox).Text = If(reader("CountryOfBirth") Is DBNull.Value, "", reader("CountryOfBirth").ToString())

                            ' Address details
                            CType(FindControlRecursive(Me, "txtStreet"), TextBox).Text = reader("StreetHouseLandmark").ToString()
                            CType(FindControlRecursive(Me, "txtLocality"), TextBox).Text = reader("AreaLocality").ToString()
                            CType(FindControlRecursive(Me, "txtTown"), TextBox).Text = reader("LocationVillageTown").ToString()
                            CType(FindControlRecursive(Me, "txtPostOffice"), TextBox).Text = reader("PostOffice").ToString()
                            CType(FindControlRecursive(Me, "txtCity"), TextBox).Text = reader("CityDistrict").ToString()
                            CType(FindControlRecursive(Me, "ddlState"), DropDownList).SelectedValue = reader("State").ToString()
                            CType(FindControlRecursive(Me, "txtCountry"), TextBox).Text = reader("Country").ToString()
                            CType(FindControlRecursive(Me, "txtPincode"), TextBox).Text = reader("Pincode").ToString()
                            CType(FindControlRecursive(Me, "ddlAddressType"), DropDownList).SelectedValue = reader("TypeOfAddress").ToString()

                            ' Same address radios
                            Dim rdoSameYes As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoSameYes"), HtmlInputRadioButton)
                            Dim rdoSameNo As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoSameNo"), HtmlInputRadioButton)
                            If rdoSameYes IsNot Nothing AndAlso rdoSameNo IsNot Nothing Then
                                rdoSameYes.Checked = False : rdoSameNo.Checked = False
                                If reader("IsPermanentAddressSame").ToString() = "Yes" Then
                                    rdoSameYes.Checked = True
                                Else
                                    rdoSameNo.Checked = True
                                    CType(FindControlRecursive(Me, "txtPermanentAddress"), TextBox).Text = reader("PermanentAddress").ToString()
                                End If
                            End If

                            ' Employment Details
                            CType(FindControlRecursive(Me, "ddlOccupation"), DropDownList).SelectedValue = reader("OccupationType").ToString()
                            CType(FindControlRecursive(Me, "txtEmployerName"), TextBox).Text = If(reader("EmployerName") Is DBNull.Value, "", reader("EmployerName").ToString())
                            CType(FindControlRecursive(Me, "txtBusinessNature"), TextBox).Text = If(reader("NatureOfBusiness") Is DBNull.Value, "", reader("NatureOfBusiness").ToString())
                            CType(FindControlRecursive(Me, "txtDesignation"), TextBox).Text = If(reader("Designation") Is DBNull.Value, "", reader("Designation").ToString())
                            CType(FindControlRecursive(Me, "ddlIncomeRange"), DropDownList).SelectedValue = reader("AnnualIncomeRange").ToString()

                            ' Source of funds
                            Dim rdoFundsSalary As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoFundsSalary"), HtmlInputRadioButton)
                            Dim rdoFundsBusiness As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoFundsBusiness"), HtmlInputRadioButton)
                            Dim rdoFundsInvestments As HtmlInputRadioButton = CType(FindControlRecursive(Me, "rdoFundsInvestments"), HtmlInputRadioButton)
                            If rdoFundsSalary IsNot Nothing AndAlso rdoFundsBusiness IsNot Nothing AndAlso rdoFundsInvestments IsNot Nothing Then
                                rdoFundsSalary.Checked = False : rdoFundsBusiness.Checked = False : rdoFundsInvestments.Checked = False
                                If reader("SourceOfFunds").ToString() = "Salary" Then
                                    rdoFundsSalary.Checked = True
                                ElseIf reader("SourceOfFunds").ToString() = "Business" Then
                                    rdoFundsBusiness.Checked = True
                                ElseIf reader("SourceOfFunds").ToString() = "Investments" Then
                                    rdoFundsInvestments.Checked = True
                                End If
                            End If

                            ' ID Proofs
                            CType(FindControlRecursive(Me, "txtPANNumber"), TextBox).Text = reader("PANNumber").ToString()
                            CType(FindControlRecursive(Me, "txtPANHolderName"), TextBox).Text = reader("PANHolderName").ToString()
                            CType(FindControlRecursive(Me, "txtDLNumber"), TextBox).Text = If(reader("DrivingLicenceNumber") Is DBNull.Value, "", reader("DrivingLicenceNumber").ToString())
                            CType(FindControlRecursive(Me, "txtDLDOB"), TextBox).Text = If(reader("DrivingLicenceDOB") Is DBNull.Value, "", Convert.ToDateTime(reader("DrivingLicenceDOB")).ToString("yyyy-MM-dd"))
                            CType(FindControlRecursive(Me, "txtDLName"), TextBox).Text = If(reader("DrivingLicenceName") Is DBNull.Value, "", reader("DrivingLicenceName").ToString())

                            ' Inject beautiful dynamic feedback script to visual dropzones (Zero placeholders)
                            Dim previewScript As String = String.Format(
                                "<script type='text/javascript'>" & vbCrLf &
                                "    window.addEventListener('DOMContentLoaded', () => {{" & vbCrLf &
                                "        document.getElementById('zoneAadhaar').classList.add('has-file');" & vbCrLf &
                                "        document.getElementById('previewAadhaar').innerHTML = '<span class=""text-success fw-bold""><i class=""bi bi-check-circle-fill me-1""></i>Aadhaar Uploaded</span>';" & vbCrLf &
                                "        document.getElementById('zonePAN').classList.add('has-file');" & vbCrLf &
                                "        document.getElementById('previewPAN').innerHTML = '<span class=""text-success fw-bold""><i class=""bi bi-check-circle-fill me-1""></i>PAN Uploaded</span>';" & vbCrLf &
                                "        document.getElementById('zoneSignature').classList.add('has-file');" & vbCrLf &
                                "        document.getElementById('previewSignature').innerHTML = '<span class=""text-success fw-bold""><i class=""bi bi-check-circle-fill me-1""></i>Signature Uploaded</span>';" & vbCrLf &
                                "        if('{0}' !== '') {{" & vbCrLf &
                                "            document.getElementById('zonePassportDL').classList.add('has-file');" & vbCrLf &
                                "            document.getElementById('previewPassportDL').innerHTML = '<span class=""text-success fw-bold""><i class=""bi bi-check-circle-fill me-1""></i>Passport/DL Uploaded</span>';" & vbCrLf &
                                "        }}" & vbCrLf &
                                "        if('{1}' !== '') {{" & vbCrLf &
                                "            document.getElementById('zoneAddressProof').classList.add('has-file');" & vbCrLf &
                                "            document.getElementById('previewAddressProof').innerHTML = '<span class=""text-success fw-bold""><i class=""bi bi-check-circle-fill me-1""></i>Address Proof Uploaded</span>';" & vbCrLf &
                                "        }}" & vbCrLf &
                                "    }});" & vbCrLf &
                                "</script>",
                                If(reader("PassportDLPath") Is DBNull.Value, "", "loaded"),
                                If(reader("AddressProofPath") Is DBNull.Value, "", "loaded")
                            )
                            Dim litServerToasts As Literal = CType(FindControlRecursive(Me, "litServerToasts"), Literal)
                            If litServerToasts IsNot Nothing Then
                                litServerToasts.Text = previewScript
                            End If
                        Else
                            InjectServerToast("Profile Not Found", "The requested KYC ID does not exist in our core database.", "danger")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            InjectServerToast("Database Error", "Failed to load record details: " & ex.Message.Replace("'", "\'"), "danger")
        End Try
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

            ' Check Edit Mode State
            Dim isEditMode As Boolean = False
            Dim editId As Integer = 0
            Dim hdnEditId As HiddenField = CType(FindControlRecursive(Me, "hdnEditId"), HiddenField)
            If hdnEditId IsNot Nothing AndAlso Not String.IsNullOrEmpty(hdnEditId.Value) Then
                If Integer.TryParse(hdnEditId.Value, editId) Then
                    isEditMode = True
                End If
            End If

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

            Dim duplicateMsg As String = CheckDuplicates(cleanAadhaar, cleanPAN, If(isEditMode, editId, 0))
            If Not String.IsNullOrEmpty(duplicateMsg) Then
                InjectServerToast("Profile Conflict", duplicateMsg, "danger")
                Return
            End If

            ' 4. Server-Side File Upload Streams Validation (Format & Size <= 2MB)
            If Not ValidateAndSaveFiles(fileAadhaar, filePAN, filePassportDL, fileAddressProof, fileSignature, isEditMode, _
                                         outAadhaarPath, outPANPath, outDLPath, outAddrPath, outSignPath, errorMsg) Then
                InjectServerToast("Upload Failure", errorMsg, "danger")
                Return
            End If

            ' 5. Perform Secure Database Insertion or Updating using Compiled Stored Procedures
            Dim connString As String = WebConfigurationManager.ConnectionStrings(CONNECTION_STRING_KEY).ConnectionString

            Using conn As New SqlConnection(connString)
                Dim procName As String = If(isEditMode, "sp_UpdateKYCRecord", "sp_InsertKYCRecord")
                Using cmd As New SqlCommand(procName, conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    ' Bind all parameters safely
                    cmd.Parameters.AddWithValue("@AcType", ddlAcType.SelectedValue)
                    cmd.Parameters.AddWithValue("@CustType", custType)
                    cmd.Parameters.AddWithValue("@Branch", ddlPrefBranch.SelectedValue)
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

                    If isEditMode Then
                        cmd.Parameters.AddWithValue("@Id", editId)
                    Else
                        cmd.Parameters.AddWithValue("@AppDate", DateTime.Today)
                    End If

                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' 6. Redirect via Post-Redirect-Get (PRG) pattern - set to True for secure thread termination
            If isEditMode Then
                Response.Redirect("ManageKYC.aspx?update=1", True)
            Else
                Response.Redirect("Default.aspx?success=1", True)
            End If

        Catch ex As System.Threading.ThreadAbortException
            ' Normal behavior during Redirect(..., True)
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
    Private Function ValidateAndSaveFiles(ByVal fAadhaar As FileUpload, ByVal fPAN As FileUpload, ByVal fDL As FileUpload, ByVal fAddr As FileUpload, ByVal fSign As FileUpload, ByVal isEdit As Boolean, ByRef pathAadhaar As String, ByRef pathPAN As String, ByRef pathDL As String, ByRef pathAddr As String, ByRef pathSign As String, ByRef errOut As String) As Boolean
        Try
            ' Create Uploads directory in root if it does not exist
            Dim uploadFolder As String = Server.MapPath("~/Uploads")
            If Not Directory.Exists(uploadFolder) Then
                Directory.CreateDirectory(uploadFolder)
            End If

            ' Mandatory uploads check (only if not editing)
            If Not isEdit Then
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
            End If

            ' Process files - if in edit mode and no file is chosen, we keep the original path (pre-filled from hidden fields)
            If fAadhaar IsNot Nothing AndAlso fAadhaar.HasFile Then
                If Not SaveSingleFile(fAadhaar, "Aadhaar", uploadFolder, pathAadhaar, errOut) Then Return False
            ElseIf isEdit Then
                Dim hdnAadhaarPath As HiddenField = CType(FindControlRecursive(Me, "hdnAadhaarPath"), HiddenField)
                If hdnAadhaarPath IsNot Nothing Then pathAadhaar = hdnAadhaarPath.Value
            End If

            If fPAN IsNot Nothing AndAlso fPAN.HasFile Then
                If Not SaveSingleFile(fPAN, "PAN", uploadFolder, pathPAN, errOut) Then Return False
            ElseIf isEdit Then
                Dim hdnPANPath As HiddenField = CType(FindControlRecursive(Me, "hdnPANPath"), HiddenField)
                If hdnPANPath IsNot Nothing Then pathPAN = hdnPANPath.Value
            End If

            If fDL IsNot Nothing AndAlso fDL.HasFile Then
                If Not SaveSingleFile(fDL, "PassportDL", uploadFolder, pathDL, errOut) Then Return False
            ElseIf isEdit Then
                Dim hdnDLPath As HiddenField = CType(FindControlRecursive(Me, "hdnDLPath"), HiddenField)
                If hdnDLPath IsNot Nothing Then pathDL = hdnDLPath.Value
            Else
                pathDL = ""
            End If

            If fAddr IsNot Nothing AndAlso fAddr.HasFile Then
                If Not SaveSingleFile(fAddr, "AddressProof", uploadFolder, pathAddr, errOut) Then Return False
            ElseIf isEdit Then
                Dim hdnAddrPath As HiddenField = CType(FindControlRecursive(Me, "hdnAddrPath"), HiddenField)
                If hdnAddrPath IsNot Nothing Then pathAddr = hdnAddrPath.Value
            Else
                pathAddr = ""
            End If

            If fSign IsNot Nothing AndAlso fSign.HasFile Then
                If Not SaveSingleFile(fSign, "Signature", uploadFolder, pathSign, errOut) Then Return False
            ElseIf isEdit Then
                Dim hdnSignPath As HiddenField = CType(FindControlRecursive(Me, "hdnSignPath"), HiddenField)
                If hdnSignPath IsNot Nothing Then pathSign = hdnSignPath.Value
            End If

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
    Private Function CheckDuplicates(ByVal aadhaar As String, ByVal pan As String, Optional ByVal excludeId As Integer = 0) As String
        Dim connString As String = WebConfigurationManager.ConnectionStrings(CONNECTION_STRING_KEY).ConnectionString
        Dim query As String = "SELECT " & _
            "SUM(CASE WHEN AadhaarNumber = @Aadhaar AND Id <> @ExcludeId THEN 1 ELSE 0 END) As AadhaarCount, " & _
            "SUM(CASE WHEN PANNumber = @PAN AND Id <> @ExcludeId THEN 1 ELSE 0 END) As PANCount " & _
            "FROM KYCDetails"

        Try
            Using conn As New SqlConnection(connString)
                Using cmd As New SqlCommand("sp_CheckKYCDuplicates", conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@Aadhaar", aadhaar)
                    cmd.Parameters.AddWithValue("@PAN", pan)
                    cmd.Parameters.AddWithValue("@ExcludeId", excludeId)

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
                    "[CreatedAt] DATETIME DEFAULT GETDATE(), " & _
                    "[VerificationStatus] NVARCHAR(20) DEFAULT 'Pending' NOT NULL" & _
                    ");"
                Using cmd As New SqlCommand(tblQuery, conn)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using

                Dim alterQuery As String = "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[KYCDetails]') AND name = 'VerificationStatus') " & _
                    "ALTER TABLE [dbo].[KYCDetails] ADD [VerificationStatus] NVARCHAR(20) DEFAULT 'Pending' NOT NULL;"
                Using cmdAlter As New SqlCommand(alterQuery, conn)
                    cmdAlter.ExecuteNonQuery()
                End Using

                ' Dynamically compile all 7 Stored Procedures inside local SQL Server DB
                BootstrapStoredProcedures(conn)
            End Using
        Catch ex As Exception
            Throw New Exception("Failed to bootstrap table structures and Stored Procedures on KYCDB: " & ex.Message, ex)
        End Try
    End Sub

    ''' <summary>
    ''' Compiles all necessary compiled procedures inside the database dynamically.
    ''' </summary>
    Private Sub BootstrapStoredProcedures(ByVal conn As SqlConnection)
        ' 1. sp_InsertKYCRecord
        CreateSPIfMissing(conn, "sp_InsertKYCRecord", _
            "CREATE PROCEDURE [dbo].[sp_InsertKYCRecord] " & vbCrLf & _
            "    @AcType NVARCHAR(50), @CustType NVARCHAR(50), @Branch NVARCHAR(100), @AppDate DATE, @Email NVARCHAR(150), @EmailOTP NVARCHAR(10), " & vbCrLf & _
            "    @Mobile NVARCHAR(15), @AltMobile NVARCHAR(15), @AadhaarMobileOTP NVARCHAR(10), @AadhaarNum NVARCHAR(20), @AadhaarOTP NVARCHAR(10), " & vbCrLf & _
            "    @AadhaarName NVARCHAR(100), @AadhaarDOB DATE, @Gender NVARCHAR(20), @FullName NVARCHAR(100), @FatherName NVARCHAR(100), " & vbCrLf & _
            "    @MotherName NVARCHAR(100), @Spouse NVARCHAR(100), @Marital NVARCHAR(50), @Nat NVARCHAR(50), @Rel NVARCHAR(50), @ResStatus NVARCHAR(50), " & vbCrLf & _
            "    @PlaceOfBirth NVARCHAR(100), @CountryOfBirth NVARCHAR(100), @Street NVARCHAR(250), @Locality NVARCHAR(150), @Town NVARCHAR(150), " & vbCrLf & _
            "    @PO NVARCHAR(100), @City NVARCHAR(100), @State NVARCHAR(100), @Country NVARCHAR(100), @Pin NVARCHAR(10), @AddrType NVARCHAR(50), " & vbCrLf & _
            "    @IsSame NVARCHAR(5), @PermAddr NVARCHAR(500), @Occ NVARCHAR(50), @EmpName NVARCHAR(150), @BusNature NVARCHAR(150), @Role NVARCHAR(100), " & vbCrLf & _
            "    @Income NVARCHAR(100), @Funds NVARCHAR(100), @PAN NVARCHAR(20), @PANName NVARCHAR(100), @DLNum NVARCHAR(30), @DLDOB DATE, @DLName NVARCHAR(100), " & vbCrLf & _
            "    @AadhaarPath NVARCHAR(500), @PANPath NVARCHAR(500), @DLPath NVARCHAR(500), @AddrPath NVARCHAR(500), @SignPath NVARCHAR(500) " & vbCrLf & _
            "AS " & vbCrLf & _
            "BEGIN " & vbCrLf & _
            "    SET NOCOUNT ON; " & vbCrLf & _
            "    INSERT INTO [dbo].[KYCDetails] ( " & vbCrLf & _
            "        [AccountType], [CustomerType], [PreferredBranch], [ApplicationDate], [Email], [EmailOTP], [MobileNumber], " & vbCrLf & _
            "        [AlternateMobileNumber], [AadhaarMobileOTP], [AadhaarNumber], [AadhaarOTP], [AadhaarName], [AadhaarDOB], [Gender], " & vbCrLf & _
            "        [FullLegalName], [FatherName], [MotherName], [SpouseGuardianName], [MaritalStatus], [Nationality], [Religion], " & vbCrLf & _
            "        [ResidentialStatus], [PlaceOfBirth], [CountryOfBirth], [StreetHouseLandmark], [AreaLocality], [LocationVillageTown], " & vbCrLf & _
            "        [PostOffice], [CityDistrict], [State], [Country], [Pincode], [TypeOfAddress], [IsPermanentAddressSame], [PermanentAddress], " & vbCrLf & _
            "        [OccupationType], [EmployerName], [NatureOfBusiness], [Designation], [AnnualIncomeRange], [SourceOfFunds], " & vbCrLf & _
            "        [PANNumber], [PANHolderName], [DrivingLicenceNumber], [DrivingLicenceDOB], [DrivingLicenceName], " & vbCrLf & _
            "        [AadhaarCardPath], [PANCardPath], [PassportDLPath], [AddressProofPath], [SignatureScanPath] " & vbCrLf & _
            "    ) VALUES ( " & vbCrLf & _
            "        @AcType, @CustType, @Branch, @AppDate, @Email, @EmailOTP, @Mobile, " & vbCrLf & _
            "        @AltMobile, @AadhaarMobileOTP, @AadhaarNum, @AadhaarOTP, @AadhaarName, @AadhaarDOB, @Gender, " & vbCrLf & _
            "        @FullName, @FatherName, @MotherName, @Spouse, @Marital, @Nat, @Rel, " & vbCrLf & _
            "        @ResStatus, @PlaceOfBirth, @CountryOfBirth, @Street, @Locality, @Town, " & vbCrLf & _
            "        @PO, @City, @State, @Country, @Pin, @AddrType, @IsSame, @PermAddr, " & vbCrLf & _
            "        @Occ, @EmpName, @BusNature, @Role, @Income, @Funds, " & vbCrLf & _
            "        @PAN, @PANName, @DLNum, @DLDOB, @DLName, " & vbCrLf & _
            "        @AadhaarPath, @PANPath, @DLPath, @AddrPath, @SignPath " & vbCrLf & _
            "    ); " & vbCrLf & _
            "    SELECT SCOPE_IDENTITY() AS [NewRecordId]; " & vbCrLf & _
            "END")

        ' 2. sp_UpdateKYCRecord
        CreateSPIfMissing(conn, "sp_UpdateKYCRecord", _
            "CREATE PROCEDURE [dbo].[sp_UpdateKYCRecord] " & vbCrLf & _
            "    @Id INT, @AcType NVARCHAR(50), @CustType NVARCHAR(50), @Branch NVARCHAR(100), @Email NVARCHAR(150), @EmailOTP NVARCHAR(10), " & vbCrLf & _
            "    @Mobile NVARCHAR(15), @AltMobile NVARCHAR(15), @AadhaarMobileOTP NVARCHAR(10), @AadhaarNum NVARCHAR(20), @AadhaarOTP NVARCHAR(10), " & vbCrLf & _
            "    @AadhaarName NVARCHAR(100), @AadhaarDOB DATE, @Gender NVARCHAR(20), @FullName NVARCHAR(100), @FatherName NVARCHAR(100), " & vbCrLf & _
            "    @MotherName NVARCHAR(100), @Spouse NVARCHAR(100), @Marital NVARCHAR(50), @Nat NVARCHAR(50), @Rel NVARCHAR(50), @ResStatus NVARCHAR(50), " & vbCrLf & _
            "    @PlaceOfBirth NVARCHAR(100), @CountryOfBirth NVARCHAR(100), @Street NVARCHAR(250), @Locality NVARCHAR(150), @Town NVARCHAR(150), " & vbCrLf & _
            "    @PO NVARCHAR(100), @City NVARCHAR(100), @State NVARCHAR(100), @Country NVARCHAR(100), @Pin NVARCHAR(10), @AddrType NVARCHAR(50), " & vbCrLf & _
            "    @IsSame NVARCHAR(5), @PermAddr NVARCHAR(500), @Occ NVARCHAR(50), @EmpName NVARCHAR(150), @BusNature NVARCHAR(150), @Role NVARCHAR(100), " & vbCrLf & _
            "    @Income NVARCHAR(100), @Funds NVARCHAR(100), @PAN NVARCHAR(20), @PANName NVARCHAR(100), @DLNum NVARCHAR(30), @DLDOB DATE, @DLName NVARCHAR(100), " & vbCrLf & _
            "    @AadhaarPath NVARCHAR(500), @PANPath NVARCHAR(500), @DLPath NVARCHAR(500), @AddrPath NVARCHAR(500), @SignPath NVARCHAR(500) " & vbCrLf & _
            "AS " & vbCrLf & _
            "BEGIN " & vbCrLf & _
            "    SET NOCOUNT ON; " & vbCrLf & _
            "    UPDATE [dbo].[KYCDetails] SET " & vbCrLf & _
            "        [AccountType] = @AcType, [CustomerType] = @CustType, [PreferredBranch] = @Branch, [Email] = @Email, [EmailOTP] = @EmailOTP, " & vbCrLf & _
            "        [MobileNumber] = @Mobile, [AlternateMobileNumber] = @AltMobile, [AadhaarMobileOTP] = @AadhaarMobileOTP, " & vbCrLf & _
            "        [AadhaarNumber] = @AadhaarNum, [AadhaarOTP] = @AadhaarOTP, [AadhaarName] = @AadhaarName, [AadhaarDOB] = @AadhaarDOB, " & vbCrLf & _
            "        [Gender] = @Gender, [FullLegalName] = @FullName, [FatherName] = @FatherName, [MotherName] = @MotherName, " & vbCrLf & _
            "        [SpouseGuardianName] = @Spouse, [MaritalStatus] = @Marital, [Nationality] = @Nat, [Religion] = @Rel, " & vbCrLf & _
            "        [ResidentialStatus] = @ResStatus, [PlaceOfBirth] = @PlaceOfBirth, [CountryOfBirth] = @CountryOfBirth, " & vbCrLf & _
            "        [StreetHouseLandmark] = @Street, [AreaLocality] = @Locality, [LocationVillageTown] = @Town, " & vbCrLf & _
            "        [PostOffice] = @PO, [CityDistrict] = @City, [State] = @State, [Country] = @Country, [Pincode] = @Pin, " & vbCrLf & _
            "        [TypeOfAddress] = @AddrType, [IsPermanentAddressSame] = @IsSame, [PermanentAddress] = @PermAddr, " & vbCrLf & _
            "        [OccupationType] = @Occ, [EmployerName] = @EmpName, [NatureOfBusiness] = @BusNature, [Designation] = @Role, " & vbCrLf & _
            "        [AnnualIncomeRange] = @Income, [SourceOfFunds] = @Funds, [PANNumber] = @PAN, [PANHolderName] = @PANName, " & vbCrLf & _
            "        [DrivingLicenceNumber] = @DLNum, [DrivingLicenceDOB] = @DLDOB, [DrivingLicenceName] = @DLName, " & vbCrLf & _
            "        [AadhaarCardPath] = @AadhaarPath, [PANCardPath] = @PANPath, [PassportDLPath] = @DLPath, " & vbCrLf & _
            "        [AddressProofPath] = @AddrPath, [SignatureScanPath] = @SignPath " & vbCrLf & _
            "    WHERE [Id] = @Id; " & vbCrLf & _
            "END")

        ' 3. sp_DeleteKYCRecord
        CreateSPIfMissing(conn, "sp_DeleteKYCRecord", _
            "CREATE PROCEDURE [dbo].[sp_DeleteKYCRecord] " & vbCrLf & _
            "    @Id INT " & vbCrLf & _
            "AS " & _
            "BEGIN " & vbCrLf & _
            "    SET NOCOUNT ON; " & vbCrLf & _
            "    SELECT [AadhaarCardPath], [PANCardPath], [PassportDLPath], [AddressProofPath], [SignatureScanPath] " & vbCrLf & _
            "    FROM [dbo].[KYCDetails] " & vbCrLf & _
            "    WHERE [Id] = @Id; " & vbCrLf & _
            "    DELETE FROM [dbo].[KYCDetails] WHERE [Id] = @Id; " & vbCrLf & _
            "END")

        ' 4. sp_GetKYCRecordById
        CreateSPIfMissing(conn, "sp_GetKYCRecordById", _
            "CREATE PROCEDURE [dbo].[sp_GetKYCRecordById] " & vbCrLf & _
            "    @Id INT " & vbCrLf & _
            "AS " & vbCrLf & _
            "BEGIN " & vbCrLf & _
            "    SET NOCOUNT ON; " & vbCrLf & _
            "    SELECT * FROM [dbo].[KYCDetails] WHERE [Id] = @Id; " & vbCrLf & _
            "END")

        ' 5. sp_SearchKYCRecords
        CreateSPIfMissing(conn, "sp_SearchKYCRecords", _
            "CREATE PROCEDURE [dbo].[sp_SearchKYCRecords] " & vbCrLf & _
            "    @Name NVARCHAR(100) = NULL, " & vbCrLf & _
            "    @Aadhaar NVARCHAR(20) = NULL, " & vbCrLf & _
            "    @PAN NVARCHAR(20) = NULL, " & vbCrLf & _
            "    @Mobile NVARCHAR(15) = NULL " & vbCrLf & _
            "AS " & vbCrLf & _
            "BEGIN " & vbCrLf & _
            "    SET NOCOUNT ON; " & vbCrLf & _
            "    SELECT [Id], [FullLegalName], [AadhaarNumber], [PANNumber], [MobileNumber], [ApplicationDate], [VerificationStatus] " & vbCrLf & _
            "    FROM [dbo].[KYCDetails] " & vbCrLf & _
            "    WHERE (@Name IS NULL OR [FullLegalName] LIKE '%' + @Name + '%') " & vbCrLf & _
            "      AND (@Aadhaar IS NULL OR [AadhaarNumber] = @Aadhaar) " & vbCrLf & _
            "      AND (@PAN IS NULL OR [PANNumber] = @PAN) " & vbCrLf & _
            "      AND (@Mobile IS NULL OR [MobileNumber] = @Mobile) " & vbCrLf & _
            "    ORDER BY [CreatedAt] DESC; " & vbCrLf & _
            "END")

        ' 6. sp_UpdateVerificationStatus
        CreateSPIfMissing(conn, "sp_UpdateVerificationStatus", _
            "CREATE PROCEDURE [dbo].[sp_UpdateVerificationStatus] " & vbCrLf & _
            "    @Id INT, " & vbCrLf & _
            "    @Status NVARCHAR(20) " & vbCrLf & _
            "AS " & vbCrLf & _
            "BEGIN " & vbCrLf & _
            "    SET NOCOUNT ON; " & vbCrLf & _
            "    UPDATE [dbo].[KYCDetails] SET [VerificationStatus] = @Status WHERE [Id] = @Id; " & vbCrLf & _
            "END")

        ' 7. sp_CheckKYCDuplicates
        CreateSPIfMissing(conn, "sp_CheckKYCDuplicates", _
            "CREATE PROCEDURE [dbo].[sp_CheckKYCDuplicates] " & vbCrLf & _
            "    @Aadhaar NVARCHAR(20), " & vbCrLf & _
            "    @PAN NVARCHAR(20), " & vbCrLf & _
            "    @ExcludeId INT = 0 " & vbCrLf & _
            "AS " & vbCrLf & _
            "BEGIN " & vbCrLf & _
            "    SET NOCOUNT ON; " & vbCrLf & _
            "    SELECT " & vbCrLf & _
            "        SUM(CASE WHEN [AadhaarNumber] = @Aadhaar AND [Id] <> @ExcludeId THEN 1 ELSE 0 END) As AadhaarCount, " & vbCrLf & _
            "        SUM(CASE WHEN [PANNumber] = @PAN AND [Id] <> @ExcludeId THEN 1 ELSE 0 END) As PANCount " & vbCrLf & _
            "    FROM [dbo].[KYCDetails]; " & vbCrLf & _
            "END")
    End Sub

    Private Sub CreateSPIfMissing(ByVal conn As SqlConnection, ByVal spName As String, ByVal createSql As String)
        Dim checkQuery As String = String.Format("SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'P', N'PC')", spName)
        Dim exists As Boolean = False
        Using cmdCheck As New SqlCommand(checkQuery, conn)
            exists = Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0
        End Using
        If Not exists Then
            Using cmdCreate As New SqlCommand(createSql, conn)
                cmdCreate.ExecuteNonQuery()
            End Using
        End If
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
