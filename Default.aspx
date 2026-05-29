<%@ Page Language="vb" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Extended KYC Verification - Secure Digital Portal</title>
    
    <!-- Bootstrap 5 CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Bootstrap Icons -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css" rel="stylesheet" />
    <!-- Custom Premium Stylesheet -->
    <link href="css/style.css" rel="stylesheet" />
</head>
<body>
    <form id="kycForm" runat="server" enctype="multipart/form-data">
        <!-- Place for server-side code-behind to dynamically inject toasts -->
        <asp:Literal ID="litServerToasts" runat="server"></asp:Literal>

        <!-- Hidden fields for Edit Mode State tracking -->
        <asp:HiddenField ID="hdnEditId" runat="server" Value="" />
        <asp:HiddenField ID="hdnAadhaarPath" runat="server" Value="" />
        <asp:HiddenField ID="hdnPANPath" runat="server" Value="" />
        <asp:HiddenField ID="hdnDLPath" runat="server" Value="" />
        <asp:HiddenField ID="hdnAddrPath" runat="server" Value="" />
        <asp:HiddenField ID="hdnSignPath" runat="server" Value="" />

        <!-- Floating Toast Container for Premium Notifications -->
        <div class="toast-container position-fixed top-0 end-0 p-3" style="z-index: 9999;" id="toastContainer"></div>

        <div class="kyc-container">
            
            <!-- Elegant Header Brand -->
            <div class="brand-header">
                <h1>Secure Digital KYC Portal</h1>
                <p>Please fill out the mandatory (<span class="text-danger">*</span>) fields accurately. This details are required for identity verification, regulatory compliance, and account setup.</p>
            </div>

            <!-- Edit Mode Banner -->
            <asp:PlaceHolder ID="pnlEditMode" runat="server" Visible="false">
                <div class="alert alert-warning d-flex align-items-center justify-content-between mb-4 border-0 shadow-sm" style="background: rgba(245, 158, 11, 0.15); border-radius: 12px; color: var(--slate-700); padding: 1rem 1.5rem;">
                    <div class="d-flex align-items-center gap-3">
                        <i class="bi bi-pencil-square fs-4 text-warning"></i>
                        <div>
                            <h6 class="fw-bold mb-0">Admin Edit Mode Active</h6>
                            <small class="opacity-75">You are currently modifying an existing KYC record (ID: <strong><asp:Label ID="lblEditId" runat="server" /></strong>). Saving will update the database directly.</small>
                        </div>
                    </div>
                    <a href="ManageKYC.aspx" class="btn btn-sm btn-outline-warning border-2 fw-semibold px-3" style="border-radius: 8px;">Cancel Edit</a>
                </div>
            </asp:PlaceHolder>

            <!-- Client-side alerts for interactive actions (Simulated OTPs) -->
            <div id="alertPlaceholder"></div>

            <!-- ================== SECTION 1: BASIC ACCOUNT INFORMATION ================== -->
            <div class="section-card">
                <div class="section-title">
                    <i class="bi bi-wallet2"></i>
                    <span>SECTION 1: Basic Account Information</span>
                </div>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label" for="ddlAccountType">Account Type<span class="required-star">*</span></label>
                        <asp:DropDownList ID="ddlAccountType" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Select Account Type --</asp:ListItem>
                            <asp:ListItem Value="Savings">Savings Account</asp:ListItem>
                            <asp:ListItem Value="Current">Current Account</asp:ListItem>
                            <asp:ListItem Value="FixedDeposit">Fixed Deposit</asp:ListItem>
                            <asp:ListItem Value="Recurring">Recurring Deposit</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    
                    <div class="col-md-6">
                        <label class="form-label">Customer Type<span class="required-star">*</span></label>
                        <div class="custom-radio-group">
                            <label class="custom-radio">
                                <input type="radio" id="rdoIndividual" name="CustomerType" runat="server" checked="true" /> Individual
                            </label>
                            <label class="custom-radio">
                                <input type="radio" id="rdoNonIndividual" name="CustomerType" runat="server" /> Non-Individual
                            </label>
                        </div>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="ddlBranch">Preferred Bank<span class="required-star">*</span></label>
                        <asp:DropDownList ID="ddlBranch" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Select Bank --</asp:ListItem>
                            <asp:ListItem Value="SBI">State Bank of India (SBI)</asp:ListItem>
                            <asp:ListItem Value="HDFC">HDFC Bank</asp:ListItem>
                            <asp:ListItem Value="ICICI">ICICI Bank</asp:ListItem>
                            <asp:ListItem Value="Axis">Axis Bank</asp:ListItem>
                            <asp:ListItem Value="PNB">Punjab National Bank (PNB)</asp:ListItem>
                            <asp:ListItem Value="BOB">Bank of Baroda</asp:ListItem>
                            <asp:ListItem Value="Canara">Canara Bank</asp:ListItem>
                            <asp:ListItem Value="Union">Union Bank of India</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtApplicationDate">Date of Application<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtApplicationDate" runat="server" CssClass="form-control" TextMode="Date" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- ================== SECTION 2: CONTACT & VERIFICATION ================== -->
            <div class="section-card">
                <div class="section-title">
                    <i class="bi bi-shield-check"></i>
                    <span>SECTION 2: Contact & Verification</span>
                </div>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label" for="txtEmail">E-Mail ID<span class="required-star">*</span></label>
                        <div class="otp-group">
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="name@example.com" TextMode="Email"></asp:TextBox>
                            <button type="button" class="btn btn-otp" onclick="simulateOTP('Email')">
                                <i class="bi bi-send me-1"></i>Get OTP
                            </button>
                        </div>
                    </div>
                    
                    <div class="col-md-6">
                        <label class="form-label" for="txtEmailOTP">E-Mail OTP<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtEmailOTP" runat="server" CssClass="form-control" placeholder="Enter 6-digit OTP" MaxLength="6"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtMobileNumber">Mobile Number<span class="required-star">*</span></label>
                        <div class="otp-group">
                            <div class="input-group" style="flex: 1;">
                                <span class="input-group-text bg-white" style="border-radius: 10px 0 0 10px; border: 1.5px solid var(--slate-200); border-right: none;">+91</span>
                                <asp:TextBox ID="txtMobileNumber" runat="server" CssClass="form-control" style="border-radius: 0 10px 10px 0;" placeholder="10-digit Mobile No." MaxLength="10"></asp:TextBox>
                            </div>
                            <button type="button" class="btn btn-otp" onclick="simulateOTP('Aadhaar Mobile')">
                                <i class="bi bi-send me-1"></i>Get OTP
                            </button>
                        </div>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtAadhaarMobileOTP">Mobile OTP (for Aadhaar)<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtAadhaarMobileOTP" runat="server" CssClass="form-control" placeholder="Enter 6-digit Aadhaar OTP" MaxLength="6"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtAlternateMobile">Alternate Mobile Number</label>
                        <div class="input-group">
                            <span class="input-group-text bg-white" style="border-radius: 10px 0 0 10px; border: 1.5px solid var(--slate-200); border-right: none;">+91</span>
                            <asp:TextBox ID="txtAlternateMobile" runat="server" CssClass="form-control" style="border-radius: 0 10px 10px 0;" placeholder="Optional alternate number" MaxLength="10"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ================== SECTION 3: AADHAAR (UIDAI) DETAILS ================== -->
            <div class="section-card">
                <div class="section-title">
                    <i class="bi bi-person-badge"></i>
                    <span>SECTION 3: Aadhaar (UIDAI) Details</span>
                </div>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label" for="txtAadhaarNumber">Aadhaar Number<span class="required-star">*</span></label>
                        <div class="otp-group">
                            <asp:TextBox ID="txtAadhaarNumber" runat="server" CssClass="form-control" placeholder="12-digit UID No." MaxLength="12"></asp:TextBox>
                            <button type="button" class="btn btn-otp" onclick="simulateOTP('Aadhaar UIDAI')">
                                <i class="bi bi-send me-1"></i>Get OTP
                            </button>
                        </div>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtAadhaarOTP">Aadhaar OTP<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtAadhaarOTP" runat="server" CssClass="form-control" placeholder="Enter OTP from UIDAI" MaxLength="6"></asp:TextBox>
                    </div>

                    <div class="col-md-5">
                        <label class="form-label" for="txtAadhaarName">Aadhaar Name<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtAadhaarName" runat="server" CssClass="form-control" placeholder="Full name exactly as on Aadhaar card"></asp:TextBox>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label" for="txtAadhaarDOB">Date of Birth (as per Aadhaar)<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtAadhaarDOB" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Gender<span class="required-star">*</span></label>
                        <div class="custom-radio-group">
                            <label class="custom-radio">
                                <input type="radio" id="rdoMale" name="Gender" runat="server" checked="true" /> Male
                            </label>
                            <label class="custom-radio">
                                <input type="radio" id="rdoFemale" name="Gender" runat="server" /> Female
                            </label>
                            <label class="custom-radio">
                                <input type="radio" id="rdoOther" name="Gender" runat="server" /> Other
                            </label>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ================== SECTION 4: PERSONAL INFORMATION ================== -->
            <div class="section-card">
                <div class="section-title">
                    <i class="bi bi-person-lines-fill"></i>
                    <span>SECTION 4: Personal Information</span>
                </div>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label" for="txtFullName">Full Legal Name<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Full name exactly as per ID proofs"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtFatherName">Father's Name<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtFatherName" runat="server" CssClass="form-control" placeholder="Father's full name"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtMotherName">Mother's Name<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtMotherName" runat="server" CssClass="form-control" placeholder="Mother's full name"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtSpouseGuardian">Spouse/Guardian Name (Care Of)<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtSpouseGuardian" runat="server" CssClass="form-control" placeholder="Spouse or Guardian name"></asp:TextBox>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label" for="ddlMaritalStatus">Marital Status<span class="required-star">*</span></label>
                        <asp:DropDownList ID="ddlMaritalStatus" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Select Status --</asp:ListItem>
                            <asp:ListItem Value="Single">Single</asp:ListItem>
                            <asp:ListItem Value="Married">Married</asp:ListItem>
                            <asp:ListItem Value="Widowed">Widowed</asp:ListItem>
                            <asp:ListItem Value="Divorced">Divorced</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label" for="txtNationality">Nationality<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtNationality" runat="server" CssClass="form-control" Text="Indian" placeholder="e.g. Indian"></asp:TextBox>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label" for="txtReligion">Religion</label>
                        <asp:TextBox ID="txtReligion" runat="server" CssClass="form-control" placeholder="e.g. Hinduism, Islam, etc."></asp:TextBox>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label" for="ddlResidentialStatus">Residential Status<span class="required-star">*</span></label>
                        <asp:DropDownList ID="ddlResidentialStatus" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Select Status --</asp:ListItem>
                            <asp:ListItem Value="Resident">Resident</asp:ListItem>
                            <asp:ListItem Value="NRI">NRI (Non-Resident Indian)</asp:ListItem>
                            <asp:ListItem Value="OCI">OCI (Overseas Citizen of India)</asp:ListItem>
                            <asp:ListItem Value="PIO">PIO (Person of Indian Origin)</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtPlaceOfBirth">Place of Birth</label>
                        <asp:TextBox ID="txtPlaceOfBirth" runat="server" CssClass="form-control" placeholder="City / Town / Village of birth"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtCountryOfBirth">Country of Birth</label>
                        <asp:TextBox ID="txtCountryOfBirth" runat="server" CssClass="form-control" Text="India" placeholder="Country of birth"></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- ================== SECTION 5: ADDRESS DETAILS ================== -->
            <div class="section-card">
                <div class="section-title">
                    <i class="bi bi-geo-alt"></i>
                    <span>SECTION 5: Address Details (Current / Correspondence)</span>
                </div>
                <div class="row g-3">
                    <div class="col-12">
                        <label class="form-label" for="txtStreet">Street / House / Landmark<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtStreet" runat="server" CssClass="form-control" placeholder="Flat/House No., Building, Landmark"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtLocality">Area / Locality<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtLocality" runat="server" CssClass="form-control" placeholder="Locality or Sector name"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtTown">Location / Village / Town<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtTown" runat="server" CssClass="form-control" placeholder="City or Town name"></asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label" for="txtPostOffice">Post Office<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtPostOffice" runat="server" CssClass="form-control" placeholder="Local P.O. name"></asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label" for="txtCity">City / District<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" placeholder="District name"></asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label" for="ddlState">State / UT<span class="required-star">*</span></label>
                        <asp:DropDownList ID="ddlState" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Select State / UT --</asp:ListItem>
                            <asp:ListItem Value="Andaman & Nicobar">Andaman and Nicobar Islands</asp:ListItem>
                            <asp:ListItem Value="Andhra Pradesh">Andhra Pradesh</asp:ListItem>
                            <asp:ListItem Value="Arunachal Pradesh">Arunachal Pradesh</asp:ListItem>
                            <asp:ListItem Value="Assam">Assam</asp:ListItem>
                            <asp:ListItem Value="Bihar">Bihar</asp:ListItem>
                            <asp:ListItem Value="Chandigarh">Chandigarh</asp:ListItem>
                            <asp:ListItem Value="Chhattisgarh">Chhattisgarh</asp:ListItem>
                            <asp:ListItem Value="Dadra & Nagar Haveli & Daman & Diu">Dadra & Nagar Haveli & Daman & Diu</asp:ListItem>
                            <asp:ListItem Value="Delhi">Delhi (NCT)</asp:ListItem>
                            <asp:ListItem Value="Goa">Goa</asp:ListItem>
                            <asp:ListItem Value="Gujarat">Gujarat</asp:ListItem>
                            <asp:ListItem Value="Haryana">Haryana</asp:ListItem>
                            <asp:ListItem Value="Himachal Pradesh">Himachal Pradesh</asp:ListItem>
                            <asp:ListItem Value="Jammu & Kashmir">Jammu & Kashmir</asp:ListItem>
                            <asp:ListItem Value="Jharkhand">Jharkhand</asp:ListItem>
                            <asp:ListItem Value="Karnataka">Karnataka</asp:ListItem>
                            <asp:ListItem Value="Kerala">Kerala</asp:ListItem>
                            <asp:ListItem Value="Ladakh">Ladakh</asp:ListItem>
                            <asp:ListItem Value="Lakshadweep">Lakshadweep</asp:ListItem>
                            <asp:ListItem Value="Madhya Pradesh">Madhya Pradesh</asp:ListItem>
                            <asp:ListItem Value="Maharashtra">Maharashtra</asp:ListItem>
                            <asp:ListItem Value="Manipur">Manipur</asp:ListItem>
                            <asp:ListItem Value="Meghalaya">Meghalaya</asp:ListItem>
                            <asp:ListItem Value="Mizoram">Mizoram</asp:ListItem>
                            <asp:ListItem Value="Nagaland">Nagaland</asp:ListItem>
                            <asp:ListItem Value="Odisha">Odisha</asp:ListItem>
                            <asp:ListItem Value="Puducherry">Puducherry</asp:ListItem>
                            <asp:ListItem Value="Punjab">Punjab</asp:ListItem>
                            <asp:ListItem Value="Rajasthan">Rajasthan</asp:ListItem>
                            <asp:ListItem Value="Sikkim">Sikkim</asp:ListItem>
                            <asp:ListItem Value="Tamil Nadu">Tamil Nadu</asp:ListItem>
                            <asp:ListItem Value="Telangana">Telangana</asp:ListItem>
                            <asp:ListItem Value="Tripura">Tripura</asp:ListItem>
                            <asp:ListItem Value="Uttar Pradesh">Uttar Pradesh</asp:ListItem>
                            <asp:ListItem Value="Uttarakhand">Uttarakhand</asp:ListItem>
                            <asp:ListItem Value="West Bengal">West Bengal</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label" for="txtCountry">Country<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtCountry" runat="server" CssClass="form-control" Text="India"></asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label" for="txtPincode">Pincode<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtPincode" runat="server" CssClass="form-control" placeholder="6-digit ZIP" MaxLength="6"></asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label" for="ddlAddressType">Type of Address<span class="required-star">*</span></label>
                        <asp:DropDownList ID="ddlAddressType" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Select Address Type --</asp:ListItem>
                            <asp:ListItem Value="Residential">Residential</asp:ListItem>
                            <asp:ListItem Value="Office">Office</asp:ListItem>
                            <asp:ListItem Value="Other">Other</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="col-12 mt-4 pt-3 border-top">
                        <label class="form-label d-block">Is Permanent Address Same as Current?<span class="required-star">*</span></label>
                        <div class="custom-radio-group">
                            <label class="custom-radio">
                                <input type="radio" id="rdoSameYes" name="SameAddress" runat="server" checked="true" onclick="togglePermanentAddress(true);" /> Yes (Same Address)
                            </label>
                            <label class="custom-radio">
                                <input type="radio" id="rdoSameNo" name="SameAddress" runat="server" onclick="togglePermanentAddress(false);" /> No (Different Address)
                            </label>
                        </div>
                    </div>

                    <!-- Toggleable Permanent Address Box -->
                    <div id="permanentAddressSection" class="col-12 permanent-address-box hidden-address mt-3">
                        <div class="card p-3 border-dashed" style="background-color: var(--slate-50); border: 1.5px dashed var(--slate-300); border-radius: 12px;">
                            <label class="form-label" for="txtPermanentAddress">Permanent Address<span class="required-star">*</span></label>
                            <asp:TextBox ID="txtPermanentAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Enter your full permanent address details here..."></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ================== SECTION 6: EMPLOYMENT & FINANCIALS ================== -->
            <div class="section-card">
                <div class="section-title">
                    <i class="bi bi-briefcase"></i>
                    <span>SECTION 6: Employment & Financials</span>
                </div>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label" for="ddlOccupation">Occupation Type<span class="required-star">*</span></label>
                        <asp:DropDownList ID="ddlOccupation" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Select Occupation Type --</asp:ListItem>
                            <asp:ListItem Value="Salaried">Salaried</asp:ListItem>
                            <asp:ListItem Value="Business">Business</asp:ListItem>
                            <asp:ListItem Value="Retired">Retired</asp:ListItem>
                            <asp:ListItem Value="Student">Student</asp:ListItem>
                            <asp:ListItem Value="Housewife">Housewife</asp:ListItem>
                            <asp:ListItem Value="Other">Other</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtEmployerName">Employer Name / Business Name</label>
                        <asp:TextBox ID="txtEmployerName" runat="server" CssClass="form-control" placeholder="Company or Business entity name"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtBusinessNature">Nature of Business / Industry</label>
                        <asp:TextBox ID="txtBusinessNature" runat="server" CssClass="form-control" placeholder="e.g. IT, Banking, Trade, Healthcare"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtDesignation">Designation / Role</label>
                        <asp:TextBox ID="txtDesignation" runat="server" CssClass="form-control" placeholder="e.g. Senior Consultant, Proprietor"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="ddlIncomeRange">Annual Income Range<span class="required-star">*</span></label>
                        <asp:DropDownList ID="ddlIncomeRange" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Select Annual Income --</asp:ListItem>
                            <asp:ListItem Value="Below1Lakh">Below 1 Lakh</asp:ListItem>
                            <asp:ListItem Value="1to5Lakhs">1 Lakh to 5 Lakhs</asp:ListItem>
                            <asp:ListItem Value="5to10Lakhs">5 Lakhs to 10 Lakhs</asp:ListItem>
                            <asp:ListItem Value="10to25Lakhs">10 Lakhs to 25 Lakhs</asp:ListItem>
                            <asp:ListItem Value="Above25Lakhs">Above 25 Lakhs</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label">Source of Funds<span class="required-star">*</span></label>
                        <div class="custom-radio-group">
                            <label class="custom-radio">
                                <input type="radio" id="rdoFundsSalary" name="SourceOfFunds" runat="server" checked="true" /> Salary
                            </label>
                            <label class="custom-radio">
                                <input type="radio" id="rdoFundsBusiness" name="SourceOfFunds" runat="server" /> Business
                            </label>
                            <label class="custom-radio">
                                <input type="radio" id="rdoFundsInvestments" name="SourceOfFunds" runat="server" /> Investments
                            </label>
                            <label class="custom-radio">
                                <input type="radio" id="rdoFundsOthers" name="SourceOfFunds" runat="server" /> Others
                            </label>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ================== SECTION 7: BANKING & ID DETAILS ================== -->
            <div class="section-card">
                <div class="section-title">
                    <i class="bi bi-credit-card-2-front"></i>
                    <span>SECTION 7: Banking & ID Details</span>
                </div>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label" for="txtPANNumber">PAN Number<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtPANNumber" runat="server" CssClass="form-control text-uppercase" placeholder="ABCDE1234F" MaxLength="10"></asp:TextBox>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label" for="txtPANHolderName">PAN Holder Name<span class="required-star">*</span></label>
                        <asp:TextBox ID="txtPANHolderName" runat="server" CssClass="form-control" placeholder="Full name as printed on PAN card"></asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label" for="txtDLNumber">Driving Licence Number</label>
                        <asp:TextBox ID="txtDLNumber" runat="server" CssClass="form-control text-uppercase" placeholder="DL Number (e.g. DL1420110012345)" MaxLength="16"></asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label" for="txtDLDOB">Driving Licence DOB</label>
                        <asp:TextBox ID="txtDLDOB" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label" for="txtDLName">Driving Licence Name</label>
                        <asp:TextBox ID="txtDLName" runat="server" CssClass="form-control" placeholder="Full name as printed on DL"></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- ================== SECTION 8: DOCUMENT UPLOADS ================== -->
            <div class="section-card">
                <div class="section-title">
                    <i class="bi bi-cloud-arrow-up"></i>
                    <span>SECTION 8: Document Uploads</span>
                </div>
                <div class="row g-4">
                    <!-- Aadhaar Card Upload -->
                    <div class="col-md-6">
                        <label class="form-label">Upload Aadhaar Card (Front & Back)<span class="required-star">*</span></label>
                        <asp:FileUpload ID="fileAadhaar" runat="server" style="display:none;" onchange="handleFileSelect(this, 'previewAadhaar', 'zoneAadhaar')" />
                        <div class="upload-zone" id="zoneAadhaar" onclick="document.getElementById('<%= fileAadhaar.ClientID %>').click()">
                            <i class="bi bi-file-earmark-person upload-icon"></i>
                            <span class="upload-title">Click to Upload Aadhaar</span>
                            <span class="upload-subtitle">Allowed formats: PDF, JPG, JPEG | Max size: 2MB</span>
                            <div id="previewAadhaar" class="upload-preview hidden">
                                <i class="bi bi-file-earmark-check-fill preview-icon text-success"></i>
                                <div class="preview-info">
                                    <span class="preview-filename"></span>
                                    <span class="preview-filesize"></span>
                                </div>
                                <button type="button" class="btn-remove-file" onclick="removeFile(event, 'fileAadhaar', 'previewAadhaar', 'zoneAadhaar')">
                                    <i class="bi bi-x-circle-fill"></i>
                                </button>
                            </div>
                        </div>
                    </div>

                    <!-- PAN Card Upload -->
                    <div class="col-md-6">
                        <label class="form-label">Upload PAN Card<span class="required-star">*</span></label>
                        <asp:FileUpload ID="filePAN" runat="server" style="display:none;" onchange="handleFileSelect(this, 'previewPAN', 'zonePAN')" />
                        <div class="upload-zone" id="zonePAN" onclick="document.getElementById('<%= filePAN.ClientID %>').click()">
                            <i class="bi bi-file-earmark-text upload-icon"></i>
                            <span class="upload-title">Click to Upload PAN Card</span>
                            <span class="upload-subtitle">Allowed formats: PDF, JPG, JPEG | Max size: 2MB</span>
                            <div id="previewPAN" class="upload-preview hidden">
                                <i class="bi bi-file-earmark-check-fill preview-icon text-success"></i>
                                <div class="preview-info">
                                    <span class="preview-filename"></span>
                                    <span class="preview-filesize"></span>
                                </div>
                                <button type="button" class="btn-remove-file" onclick="removeFile(event, 'filePAN', 'previewPAN', 'zonePAN')">
                                    <i class="bi bi-x-circle-fill"></i>
                                </button>
                            </div>
                        </div>
                    </div>

                    <!-- Passport / Driving Licence Upload -->
                    <div class="col-md-6">
                        <label class="form-label">Upload Passport / Driving Licence</label>
                        <asp:FileUpload ID="filePassportDL" runat="server" style="display:none;" onchange="handleFileSelect(this, 'previewPassportDL', 'zonePassportDL')" />
                        <div class="upload-zone" id="zonePassportDL" onclick="document.getElementById('<%= filePassportDL.ClientID %>').click()">
                            <i class="bi bi-card-image upload-icon"></i>
                            <span class="upload-title">Click to Upload Passport/DL</span>
                            <span class="upload-subtitle">Allowed formats: PDF, JPG, JPEG | Max size: 2MB</span>
                            <div id="previewPassportDL" class="upload-preview hidden">
                                <i class="bi bi-file-earmark-check-fill preview-icon text-success"></i>
                                <div class="preview-info">
                                    <span class="preview-filename"></span>
                                    <span class="preview-filesize"></span>
                                </div>
                                <button type="button" class="btn-remove-file" onclick="removeFile(event, 'filePassportDL', 'previewPassportDL', 'zonePassportDL')">
                                    <i class="bi bi-x-circle-fill"></i>
                                </button>
                            </div>
                        </div>
                    </div>

                    <!-- Address Proof Upload -->
                    <div class="col-md-6">
                        <label class="form-label">Upload Address Proof (if different)</label>
                        <asp:FileUpload ID="fileAddressProof" runat="server" style="display:none;" onchange="handleFileSelect(this, 'previewAddressProof', 'zoneAddressProof')" />
                        <div class="upload-zone" id="zoneAddressProof" onclick="document.getElementById('<%= fileAddressProof.ClientID %>').click()">
                            <i class="bi bi-house-door upload-icon"></i>
                            <span class="upload-title">Click to Upload Address Proof</span>
                            <span class="upload-subtitle">Allowed formats: PDF, JPG, JPEG | Max size: 2MB</span>
                            <div id="previewAddressProof" class="upload-preview hidden">
                                <i class="bi bi-file-earmark-check-fill preview-icon text-success"></i>
                                <div class="preview-info">
                                    <span class="preview-filename"></span>
                                    <span class="preview-filesize"></span>
                                </div>
                                <button type="button" class="btn-remove-file" onclick="removeFile(event, 'fileAddressProof', 'previewAddressProof', 'zoneAddressProof')">
                                    <i class="bi bi-x-circle-fill"></i>
                                </button>
                            </div>
                        </div>
                    </div>

                    <!-- Signature Scan Upload -->
                    <div class="col-md-12">
                        <label class="form-label">Upload Signature Scan<span class="required-star">*</span></label>
                        <asp:FileUpload ID="fileSignature" runat="server" style="display:none;" onchange="handleFileSelect(this, 'previewSignature', 'zoneSignature')" />
                        <div class="upload-zone" id="zoneSignature" onclick="document.getElementById('<%= fileSignature.ClientID %>').click()">
                            <i class="bi bi-pen upload-icon"></i>
                            <span class="upload-title">Click to Upload Signature Scan</span>
                            <span class="upload-subtitle">Allowed formats: PDF, JPG, JPEG | Max size: 2MB</span>
                            <div id="previewSignature" class="upload-preview hidden">
                                <i class="bi bi-file-earmark-check-fill preview-icon text-success"></i>
                                <div class="preview-info">
                                    <span class="preview-filename"></span>
                                    <span class="preview-filesize"></span>
                                </div>
                                <button type="button" class="btn-remove-file" onclick="removeFile(event, 'fileSignature', 'previewSignature', 'zoneSignature')">
                                    <i class="bi bi-x-circle-fill"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Page Buttons (Action Layout) -->
            <div class="d-flex justify-content-end gap-3 mt-4">
                <button type="button" class="btn btn-secondary-custom" onclick="resetKYCForm()">
                    <i class="bi bi-arrow-counterclockwise me-2"></i>Reset Form
                </button>
                <button type="submit" id="btnSave" runat="server" onserverclick="btnSave_Click" onclick="if(!validateKYCForm(event)) return false;" class="btn btn-primary-custom">
                    <i class="bi bi-check2-circle me-2"></i>Save
                </button>
            </div>

            <!-- Footer -->
            <div class="kyc-footer">
                <p>Digital KYC Form System v1.0 | Secure Identity Portal</p>
            </div>

        </div>
    </form>

    <!-- Bootstrap 5 JS Bundle -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>

    <!-- Client-side Interactive Script -->
    <script type="text/javascript">
        // Field registry mapped directly to exact validation rule types
        const fields = [
            { id: '<%= ddlAccountType.ClientID %>', label: 'Account Type', required: true, type: 'select' },
            { id: '<%= ddlBranch.ClientID %>', label: 'Preferred Bank', required: true, type: 'select' },
            { id: '<%= txtEmail.ClientID %>', label: 'E-Mail ID', required: true, type: 'email' },
            { id: '<%= txtEmailOTP.ClientID %>', label: 'E-Mail OTP', required: true, type: 'otp' },
            { id: '<%= txtMobileNumber.ClientID %>', label: 'Mobile Number', required: true, type: 'mobile' },
            { id: '<%= txtAlternateMobile.ClientID %>', label: 'Alternate Mobile Number', required: false, type: 'altmobile' },
            { id: '<%= txtAadhaarMobileOTP.ClientID %>', label: 'Mobile OTP (for Aadhaar)', required: true, type: 'otp' },
            { id: '<%= txtAadhaarNumber.ClientID %>', label: 'Aadhaar Number', required: true, type: 'aadhaar' },
            { id: '<%= txtAadhaarOTP.ClientID %>', label: 'Aadhaar OTP', required: true, type: 'otp' },
            { id: '<%= txtAadhaarName.ClientID %>', label: 'Aadhaar Name', required: true, type: 'name' },
            { id: '<%= txtAadhaarDOB.ClientID %>', label: 'Date of Birth (as per Aadhaar)', required: true, type: 'date' },
            { id: '<%= txtFullName.ClientID %>', label: 'Full Legal Name', required: true, type: 'name' },
            { id: '<%= txtFatherName.ClientID %>', label: 'Father\'s Name', required: true, type: 'name' },
            { id: '<%= txtMotherName.ClientID %>', label: 'Mother\'s Name', required: true, type: 'name' },
            { id: '<%= txtSpouseGuardian.ClientID %>', label: 'Spouse/Guardian Name', required: true, type: 'name' },
            { id: '<%= ddlMaritalStatus.ClientID %>', label: 'Marital Status', required: true, type: 'select' },
            { id: '<%= txtNationality.ClientID %>', label: 'Nationality', required: true, type: 'name' },
            { id: '<%= ddlResidentialStatus.ClientID %>', label: 'Residential Status', required: true, type: 'select' },
            
            // Section 5: Address Detail Fields
            { id: '<%= txtStreet.ClientID %>', label: 'Street/House/Landmark', required: true, type: 'address' },
            { id: '<%= txtLocality.ClientID %>', label: 'Area/Locality', required: true, type: 'address' },
            { id: '<%= txtTown.ClientID %>', label: 'Location/Village/Town', required: true, type: 'city' },
            { id: '<%= txtPostOffice.ClientID %>', label: 'Post Office', required: true, type: 'city' },
            { id: '<%= txtCity.ClientID %>', label: 'City/District', required: true, type: 'city' },
            { id: '<%= ddlState.ClientID %>', label: 'State', required: true, type: 'select' },
            { id: '<%= txtCountry.ClientID %>', label: 'Country', required: true, type: 'name' },
            { id: '<%= txtPincode.ClientID %>', label: 'Pincode', required: true, type: 'pincode' },
            { id: '<%= ddlAddressType.ClientID %>', label: 'Type of Address', required: true, type: 'select' },
            
            // Section 6: Employment details
            { id: '<%= ddlOccupation.ClientID %>', label: 'Occupation Type', required: true, type: 'select' },
            { id: '<%= ddlIncomeRange.ClientID %>', label: 'Annual Income Range', required: true, type: 'select' },
            
            // Section 7: Government IDs
            { id: '<%= txtPANNumber.ClientID %>', label: 'PAN Number', required: true, type: 'pan' },
            { id: '<%= txtPANHolderName.ClientID %>', label: 'PAN Holder Name', required: true, type: 'name' }
        ];

        // Toggle Permanent Address Visiblity
        function togglePermanentAddress(isSame) {
            const section = document.getElementById('permanentAddressSection');
            if (section) {
                if (isSame) {
                    section.classList.add('hidden-address');
                } else {
                    section.classList.remove('hidden-address');
                }
            }
        }

        // Simulate Sending OTP with conditional field validation first
        function simulateOTP(channel) {
            if (channel === 'Email') {
                const emailField = fields.find(f => f.id === '<%= txtEmail.ClientID %>');
                if (emailField) {
                    const isValid = validateField(emailField);
                    if (!isValid) {
                        showToast('Email Required', 'Please enter a valid E-Mail ID first before requesting an OTP.', 'danger');
                        document.getElementById(emailField.id).focus();
                        return;
                    }
                }
            } else if (channel === 'Aadhaar Mobile') {
                const mobileField = fields.find(f => f.id === '<%= txtMobileNumber.ClientID %>');
                if (mobileField) {
                    const isValid = validateField(mobileField);
                    if (!isValid) {
                        showToast('Mobile Number Required', 'Please enter a valid 10-digit Mobile Number first before requesting an OTP.', 'danger');
                        document.getElementById(mobileField.id).focus();
                        return;
                    }
                }
            } else if (channel === 'Aadhaar UIDAI') {
                const aadhaarField = fields.find(f => f.id === '<%= txtAadhaarNumber.ClientID %>');
                if (aadhaarField) {
                    const isValid = validateField(aadhaarField);
                    if (!isValid) {
                        showToast('Aadhaar Required', 'Please enter a valid 12-digit Aadhaar Number first before requesting an OTP.', 'danger');
                        document.getElementById(aadhaarField.id).focus();
                        return;
                    }
                }
            }
            
            showToast('OTP Dispatched', `A simulated 6-digit confirmation code was sent to your registered ${channel}.`, 'info');
        }

        // Floating Toast Notification Logic
        function showToast(title, message, type = 'success') {
            const container = document.getElementById('toastContainer');
            if (!container) return;
            
            const toast = document.createElement('div');
            toast.className = `toast-custom toast-${type}`;
            
            let iconClass = 'bi-patch-check-fill';
            if (type === 'danger') iconClass = 'bi-exclamation-triangle-fill';
            if (type === 'warning') iconClass = 'bi-exclamation-circle-fill';
            if (type === 'info') iconClass = 'bi-info-circle-fill';
            
            toast.innerHTML = `
                <i class="bi ${iconClass} toast-icon"></i>
                <div class="toast-content">
                    <div class="toast-title">${title}</div>
                    <div class="toast-message">${message}</div>
                </div>
                <button type="button" class="toast-close" onclick="this.parentElement.remove()">
                    <i class="bi bi-x"></i>
                </button>
            `;
            
            container.appendChild(toast);
            
            // Auto remove after 5 seconds
            setTimeout(() => {
                toast.style.animation = 'toastSlideOut 0.3s cubic-bezier(0.16, 1, 0.3, 1) forwards';
                setTimeout(() => toast.remove(), 300);
            }, 5000);
        }

        // Validation Visual Feedback State
        function setFieldState(element, isValid, errorMessage = '') {
            if (!element) return;
            
            let targetForClass = element;
            let targetForFeedback = element.parentElement;
            
            // Support nesting inside input-group or otp-group
            if (element.classList && (element.classList.contains('form-control') || element.classList.contains('form-select'))) {
                if (element.parentElement.classList.contains('input-group') || element.parentElement.classList.contains('otp-group')) {
                    targetForFeedback = element.parentElement.parentElement;
                }
            }
            
            // Support Document Upload Zone mapping
            if (element.type === 'file') {
                const fileZoneMap = {
                    '<%= fileAadhaar.ClientID %>': 'zoneAadhaar',
                    '<%= filePAN.ClientID %>': 'zonePAN',
                    '<%= filePassportDL.ClientID %>': 'zonePassportDL',
                    '<%= fileAddressProof.ClientID %>': 'zoneAddressProof',
                    '<%= fileSignature.ClientID %>': 'zoneSignature'
                };
                const zoneId = fileZoneMap[element.id];
                if (zoneId) {
                    const zone = document.getElementById(zoneId);
                    if (zone) {
                        targetForClass = zone;
                        targetForFeedback = zone.parentElement;
                    }
                }
            }
            
            // Add/Remove validation design classes
            if (isValid) {
                targetForClass.classList.remove('is-invalid');
                targetForClass.classList.add('is-valid');
            } else {
                targetForClass.classList.remove('is-valid');
                targetForClass.classList.add('is-invalid');
            }
            
            // Render error feedback message
            let feedback = targetForFeedback.querySelector('.invalid-feedback');
            if (!isValid) {
                if (!feedback) {
                    feedback = document.createElement('div');
                    feedback.className = 'invalid-feedback';
                    targetForFeedback.appendChild(feedback);
                }
                feedback.textContent = errorMessage;
                feedback.style.display = 'block';
            } else {
                if (feedback) {
                    feedback.textContent = '';
                    feedback.style.display = 'none';
                }
            }
        }

        // File Selection Checks (Format and Size - Max 2MB)
        function handleFileSelect(input, previewId, zoneId) {
            const file = input.files[0];
            const preview = document.getElementById(previewId);
            const zone = document.getElementById(zoneId);
            
            if (!file) {
                removeFile(null, input.id, previewId, zoneId);
                return;
            }
            
            // Check for duplicate files among all uploads (usability safety rule)
            const fileInputs = [
                { id: '<%= fileAadhaar.ClientID %>', label: 'Aadhaar Card' },
                { id: '<%= filePAN.ClientID %>', label: 'PAN Card' },
                { id: '<%= filePassportDL.ClientID %>', label: 'Passport/DL' },
                { id: '<%= fileAddressProof.ClientID %>', label: 'Address Proof' },
                { id: '<%= fileSignature.ClientID %>', label: 'Signature Scan' }
            ];

            for (const inputInfo of fileInputs) {
                if (inputInfo.id === input.id) continue;
                const otherInput = document.getElementById(inputInfo.id);
                if (otherInput && otherInput.files && otherInput.files.length > 0) {
                    const otherFile = otherInput.files[0];
                    if (otherFile.name === file.name && otherFile.size === file.size) {
                        showToast('Duplicate File', `You have already selected this file ("${file.name}") for the ${inputInfo.label} slot. Please select a distinct document.`, 'danger');
                        removeFile(null, input.id, previewId, zoneId);
                        setFieldState(input, false, 'Duplicate file selected.');
                        return;
                    }
                }
            }
            
            // 15. File format extensions check
            const ext = file.name.split('.').pop().toLowerCase();
            const allowed = ['pdf', 'jpg', 'jpeg'];
            
            if (!allowed.includes(ext)) {
                showToast('Format Disallowed', `Only PDF and JPG/JPEG files are accepted. Selected: .${ext}`, 'danger');
                removeFile(null, input.id, previewId, zoneId);
                setFieldState(input, false, 'Only PDF and JPG/JPEG files are allowed.');
                return;
            }
            
            // 16. File Size Validation (Max 2MB)
            const maxSize = 2 * 1024 * 1024; // 2MB
            if (file.size > maxSize) {
                showToast('File Too Large', `The file size exceeds the 2MB limit. Selected: ${(file.size / 1024 / 1024).toFixed(2)}MB`, 'danger');
                removeFile(null, input.id, previewId, zoneId);
                setFieldState(input, false, 'Maximum file size allowed is 2MB.');
                return;
            }
            
            // Update UI preview
            preview.classList.remove('hidden');
            const filenameEl = preview.querySelector('.preview-filename');
            const filesizeEl = preview.querySelector('.preview-filesize');
            const iconEl = preview.querySelector('.preview-icon');
            
            filenameEl.textContent = file.name;
            filesizeEl.textContent = (file.size / 1024 / 1024).toFixed(2) + ' MB';
            
            if (ext === 'pdf') {
                iconEl.className = 'bi bi-file-earmark-pdf preview-icon text-danger';
            } else {
                iconEl.className = 'bi bi-file-earmark-image preview-icon text-success';
            }
            
            setFieldState(input, true);
        }

        // File Removal Handler
        function removeFile(event, inputId, previewId, zoneId) {
            if (event) {
                event.stopPropagation();
                event.preventDefault();
            }
            
            const input = document.getElementById(inputId);
            const preview = document.getElementById(previewId);
            const zone = document.getElementById(zoneId);
            
            if (input) input.value = '';
            if (preview) preview.classList.add('hidden');
            
            if (zone) {
                zone.classList.remove('is-valid');
                zone.classList.remove('is-invalid');
            }
        }

        // Complete 22-Rule Mapping Single Field Validator
        function validateField(field) {
            const el = document.getElementById(field.id);
            if (!el) return true;
            
            // 21. Trim Whitespace Validation
            const val = el.value.trim();
            
            // 1. Required Field Validation
            if (field.required && !val) {
                setFieldState(el, false, `${field.label} is required.`);
                return false;
            }
            
            // 2. Dropdown validation (Ensure selection is not index 0)
            if (el.tagName === 'SELECT') {
                if (el.selectedIndex === 0 || val === "") {
                    setFieldState(el, false, `Please select a valid option for ${field.label}.`);
                    return false;
                }
            }
            
            // Optional fields check (if empty and optional, skip further checks)
            if (!field.required && !val) {
                setFieldState(el, true);
                el.classList.remove('is-valid', 'is-invalid');
                return true;
            }
            
            // 3. Email Validation
            if (field.type === 'email') {
                const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailPattern.test(val)) {
                    setFieldState(el, false, 'Please enter a valid email address (e.g. name@domain.com).');
                    return false;
                }
            }
            
            // 4. Mobile Number Validation
            if (field.type === 'mobile') {
                const mobilePattern = /^[0-9]{10}$/;
                if (!mobilePattern.test(val)) {
                    setFieldState(el, false, 'Mobile number must be exactly 10 digits (numeric only).');
                    return false;
                }
            }
            
            // 5. Alternate Mobile Validation (Only validated if filled)
            if (field.type === 'altmobile') {
                const mobilePattern = /^[0-9]{10}$/;
                if (!mobilePattern.test(val)) {
                    setFieldState(el, false, 'Alternate Mobile must be exactly 10 digits (numeric only).');
                    return false;
                }
            }
            
            // 6. OTP Validation (4-6 numeric digits)
            if (field.type === 'otp') {
                const otpPattern = /^[0-9]{4,6}$/;
                if (!otpPattern.test(val)) {
                    setFieldState(el, false, 'OTP code must be a numeric value of 4 to 6 digits.');
                    return false;
                }
            }
            
            // 7. Aadhaar Number Validation (12 digits, numeric only)
            if (field.type === 'aadhaar') {
                const aadhaarPattern = /^[0-9]{12}$/;
                if (!aadhaarPattern.test(val)) {
                    setFieldState(el, false, 'Aadhaar number must be exactly 12 digits (numeric only).');
                    return false;
                }
            }
            
            // 8. PAN Number Validation (10 characters, formatted like ABCDE1234F)
            if (field.type === 'pan') {
                const panPattern = /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/;
                if (!panPattern.test(val)) {
                    setFieldState(el, false, 'PAN must be a 10-character alphanumeric in format ABCDE1234F.');
                    return false;
                }
            }
            
            // 9 & 10. Legal Name & Aadhaar Name validation (alphabets only, spaces allowed, length 2 to 50)
            if (field.type === 'name') {
                const namePattern = /^[A-Za-z\s]{2,50}$/;
                if (!namePattern.test(val)) {
                    setFieldState(el, false, 'Name must contain only alphabets and spaces, between 2 and 50 characters.');
                    return false;
                }
            }
            
            // 11. Address validation (min length 5, no only spaces)
            if (field.type === 'address') {
                if (val.length < 5) {
                    setFieldState(el, false, `${field.label} must be at least 5 characters long.`);
                    return false;
                }
            }
            
            // 12. Pincode validation (6 digits, numeric only)
            if (field.type === 'pincode') {
                const pinPattern = /^[0-9]{6}$/;
                if (!pinPattern.test(val)) {
                    setFieldState(el, false, 'Pincode must be exactly 6 digits (numeric only).');
                    return false;
                }
            }
            
            // 14. DOB Validation (valid date, not future, min age 18)
            if (field.type === 'date' && field.id === '<%= txtAadhaarDOB.ClientID %>') {
                const dobVal = new Date(val);
                const today = new Date();
                
                if (isNaN(dobVal.getTime())) {
                    setFieldState(el, false, 'Please select a valid date.');
                    return false;
                }
                
                if (dobVal > today) {
                    setFieldState(el, false, 'Date of birth cannot be in the future.');
                    return false;
                }
                
                let age = today.getFullYear() - dobVal.getFullYear();
                const m = today.getMonth() - dobVal.getMonth();
                if (m < 0 || (m === 0 && today.getDate() < dobVal.getDate())) {
                    age--;
                }
                
                if (age < 18) {
                    setFieldState(el, false, 'Applicant must be at least 18 years old.');
                    return false;
                }
            }
            
            // 19. Prevent special characters in city/town/locality
            if (field.type === 'city') {
                const cityPattern = /^[A-Za-z\s]{2,50}$/;
                if (!cityPattern.test(val)) {
                    setFieldState(el, false, `${field.label} must contain only alphabets and spaces (2 to 50 characters).`);
                    return false;
                }
            }
            
            setFieldState(el, true);
            return true;
        }

        // Form Submit Validation Engine
        function validateKYCForm(event) {
            let formIsValid = true;
            const errors = [];
            let firstInvalidElement = null;
            
            // 1. Validate standard registered fields
            fields.forEach(field => {
                const el = document.getElementById(field.id);
                if (el) {
                    const ok = validateField(field);
                    if (!ok) {
                        formIsValid = false;
                        errors.push(field.label);
                        if (!firstInvalidElement) firstInvalidElement = el;
                    }
                }
            });
            
            // 2. Validate conditional Permanent Address if same-address is NO
            const isSameAddress = document.getElementById('<%= rdoSameYes.ClientID %>').checked;
            if (!isSameAddress) {
                const permAddressEl = document.getElementById('<%= txtPermanentAddress.ClientID %>');
                if (permAddressEl) {
                    const val = permAddressEl.value.trim();
                    if (!val) {
                        setFieldState(permAddressEl, false, 'Permanent Address is required.');
                        formIsValid = false;
                        errors.push('Permanent Address');
                        if (!firstInvalidElement) firstInvalidElement = permAddressEl;
                    } else if (val.length < 5) {
                        setFieldState(permAddressEl, false, 'Permanent Address must be at least 5 characters.');
                        formIsValid = false;
                        errors.push('Permanent Address Length');
                        if (!firstInvalidElement) firstInvalidElement = permAddressEl;
                    } else {
                        setFieldState(permAddressEl, true);
                    }
                }
            }
            
            // 3. Validate Mandatory File Uploads (Aadhaar, PAN, Signature)
            const isEditMode = document.getElementById('<%= hdnEditId.ClientID %>').value !== '';
            const mandatoryFiles = [
                { id: '<%= fileAadhaar.ClientID %>', label: 'Aadhaar Card File' },
                { id: '<%= filePAN.ClientID %>', label: 'PAN Card File' },
                { id: '<%= fileSignature.ClientID %>', label: 'Signature Scan File' }
            ];
            
            mandatoryFiles.forEach(fileField => {
                const input = document.getElementById(fileField.id);
                if (input) {
                    if (input.files.length === 0) {
                        if (!isEditMode) {
                            setFieldState(input, false, `${fileField.label} is required.`);
                            formIsValid = false;
                            errors.push(fileField.label);
                            if (!firstInvalidElement) {
                                const fileZoneMap = {
                                    '<%= fileAadhaar.ClientID %>': 'zoneAadhaar',
                                    '<%= filePAN.ClientID %>': 'zonePAN',
                                    '<%= fileSignature.ClientID %>': 'zoneSignature'
                                };
                                firstInvalidElement = document.getElementById(fileZoneMap[fileField.id]);
                            }
                        } else {
                            setFieldState(input, true); // Mark as valid in Edit Mode since it exists
                        }
                    } else {
                        if (input.classList.contains('is-invalid')) {
                            formIsValid = false;
                            errors.push(`${fileField.label} (Invalid Format or Size)`);
                            if (!firstInvalidElement) firstInvalidElement = input;
                        }
                    }
                }
            });
            
            // Visual Results handling
            if (!formIsValid) {
                if (event) {
                    event.preventDefault();
                }
                showToast('Form Verification Failed', `Please correct the highlighted fields and try again.`, 'danger');
                
                // Focus and scroll to first error control
                if (firstInvalidElement) {
                    firstInvalidElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
                    setTimeout(() => {
                        if (typeof firstInvalidElement.focus === 'function') {
                            firstInvalidElement.focus();
                        }
                    }, 500);
                }
                return false;
            }
            
            // If passes validation successfully
            localStorage.removeItem('kyc_form_progress');
            showToast('Verification Successful!', 'Your digital KYC document profile is fully verified and saved successfully!', 'success');
            return true;
        }

        // Reset Form Inputs with User Confirmation and clean page redirect
        function resetKYCForm() {
            const confirmReset = confirm("Are you sure you want to reset the entire form? All filled progress and uploaded files will be permanently cleared.");
            if (!confirmReset) {
                return;
            }

            try {
                // Wipe auto-save progress cache first
                localStorage.removeItem('kyc_form_progress');
            } catch (e) {
                console.error("Error wiping autosave cache:", e);
            }

            // Redirect to a pristine GET request version of the page
            // This natively wipes all form values, restores select dropdown indexes, 
            // clears file streams, and guarantees the page looks 100% brand new!
            window.location.href = 'Default.aspx';
        }

        // Realtime Input Handlers & 18. Numeric-only Key Filtering
        function initRealtimeValidation() {
            fields.forEach(field => {
                const el = document.getElementById(field.id);
                if (!el) return;
                
                // Trigger validate on blur
                el.addEventListener('blur', () => {
                    validateField(field);
                });
                
                // Capitalization dynamic rules
                if (field.type === 'pan') {
                    el.addEventListener('input', (e) => {
                        e.target.value = e.target.value.toUpperCase();
                    });
                }
                
                // 18. Numbers-only key filtering to prevent letters
                if (field.type === 'pincode' || field.type === 'mobile' || field.type === 'altmobile' || field.type === 'aadhaar' || field.type === 'otp') {
                    el.addEventListener('input', (e) => {
                        e.target.value = e.target.value.replace(/\D/g, '');
                    });
                }
            });
            
            // Optional DL fields validation
            const dlNumberEl = document.getElementById('<%= txtDLNumber.ClientID %>');
            if (dlNumberEl) {
                dlNumberEl.addEventListener('input', (e) => {
                    e.target.value = e.target.value.toUpperCase();
                });
                dlNumberEl.addEventListener('blur', () => {
                    const val = dlNumberEl.value.trim();
                    if (val) {
                        // Standard validation for DL format if entered
                        if (val.length < 10) {
                            setFieldState(dlNumberEl, false, 'Driving Licence number must be at least 10 characters.');
                        } else {
                            setFieldState(dlNumberEl, true);
                        }
                    } else {
                        dlNumberEl.classList.remove('is-valid', 'is-invalid');
                    }
                });
            }
        }

        // Save form progress in real-time to localStorage
        function saveKYCProgress() {
            const data = {};
            
            // Standard registered fields
            fields.forEach(field => {
                const el = document.getElementById(field.id);
                if (el) {
                    data[field.id] = el.value;
                }
            });
            
            // Additional custom text boxes
            const additionalIds = [
                '<%= txtAlternateMobile.ClientID %>',
                '<%= txtDLNumber.ClientID %>',
                '<%= txtDLDOB.ClientID %>',
                '<%= txtDLName.ClientID %>',
                '<%= txtPermanentAddress.ClientID %>'
            ];
            additionalIds.forEach(id => {
                const el = document.getElementById(id);
                if (el) {
                    data[id] = el.value;
                }
            });
            
            // Radio buttons checked state
            const radios = [
                '<%= rdoMale.ClientID %>', '<%= rdoFemale.ClientID %>', '<%= rdoOther.ClientID %>',
                '<%= rdoSameYes.ClientID %>', '<%= rdoSameNo.ClientID %>'
            ];
            radios.forEach(id => {
                const el = document.getElementById(id);
                if (el) {
                    data[id] = el.checked;
                }
            });
            
            localStorage.setItem('kyc_form_progress', JSON.stringify(data));
        }

        // Restore form progress from localStorage
        function restoreKYCProgress() {
            const raw = localStorage.getItem('kyc_form_progress');
            if (!raw) return;
            
            try {
                const data = JSON.parse(raw);
                
                // Populate elements
                for (const id in data) {
                    const el = document.getElementById(id);
                    if (!el) continue;
                    
                    if (el.type === 'radio') {
                        el.checked = data[id];
                    } else if (el.type !== 'file') {
                        el.value = data[id];
                    }
                }
                
                // Re-trigger toggle of Permanent Address
                const sameYes = document.getElementById('<%= rdoSameYes.ClientID %>');
                if (sameYes) {
                    togglePermanentAddress(sameYes.checked);
                }
            } catch (e) {
                console.error("Error restoring progress:", e);
            }
        }

        // Initial setup on Page Load
        window.addEventListener('DOMContentLoaded', () => {
            // Check if we are requesting a new, pristine form from the dashboard
            const urlParams = new URLSearchParams(window.location.search);
            if (urlParams.get('new') === '1') {
                try {
                    localStorage.removeItem('kyc_form_progress');
                } catch (e) {
                    console.error("Error wiping autosave cache:", e);
                }
                // Clean redirect to wipe search params and show pristine form
                window.location.href = 'Default.aspx';
                return;
            }

            // Restore progress before setting up validation listeners
            restoreKYCProgress();
            
            const isSame = document.getElementById('<%= rdoSameYes.ClientID %>').checked;
            togglePermanentAddress(isSame);
            initRealtimeValidation();
            
            // Listen to any changes in the form to trigger auto-saving
            const form = document.getElementById('kycForm');
            if (form) {
                form.addEventListener('input', saveKYCProgress);
                form.addEventListener('change', saveKYCProgress);
            }
        });
    </script>
</body>
</html>
