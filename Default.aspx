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
    <form id="kycForm" runat="server">
        <div class="kyc-container">
            
            <!-- Elegant Header Brand -->
            <div class="brand-header">
                <h1>Secure Digital KYC Portal</h1>
                <p>Please fill out the mandatory (<span class="text-danger">*</span>) fields accurately. This details are required for identity verification, regulatory compliance, and account setup.</p>
            </div>

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
                        <label class="form-label" for="ddlBranch">Preferred Branch<span class="required-star">*</span></label>
                        <asp:DropDownList ID="ddlBranch" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Select Branch --</asp:ListItem>
                            <asp:ListItem Value="Main">Main Head Branch</asp:ListItem>
                            <asp:ListItem Value="Downtown">Downtown Tech Park Branch</asp:ListItem>
                            <asp:ListItem Value="North">North Hub Station Branch</asp:ListItem>
                            <asp:ListItem Value="South">South Coastal Plaza Branch</asp:ListItem>
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
                        <label class="form-label" for="ddlState">State<span class="required-star">*</span></label>
                        <asp:DropDownList ID="ddlState" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Select State --</asp:ListItem>
                            <asp:ListItem Value="Andhra Pradesh">Andhra Pradesh</asp:ListItem>
                            <asp:ListItem Value="Bihar">Bihar</asp:ListItem>
                            <asp:ListItem Value="Delhi">Delhi</asp:ListItem>
                            <asp:ListItem Value="Gujarat">Gujarat</asp:ListItem>
                            <asp:ListItem Value="Karnataka">Karnataka</asp:ListItem>
                            <asp:ListItem Value="Maharashtra">Maharashtra</asp:ListItem>
                            <asp:ListItem Value="Tamil Nadu">Tamil Nadu</asp:ListItem>
                            <asp:ListItem Value="Uttar Pradesh">Uttar Pradesh</asp:ListItem>
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

            <!-- Page Buttons (Action Layout) -->
            <div class="d-flex justify-content-end gap-3 mt-4">
                <button type="button" class="btn btn-secondary-custom">
                    <i class="bi bi-arrow-counterclockwise me-2"></i>Reset Form
                </button>
                <button type="button" class="btn btn-primary-custom" onclick="alert('Form progress saved successfully!');">
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
        // Toggle Permanent Address Visiblity
        function togglePermanentAddress(isSame) {
            const section = document.getElementById('permanentAddressSection');
            if (isSame) {
                section.classList.add('hidden-address');
            } else {
                section.classList.remove('hidden-address');
            }
        }

        // Simulate Sending OTP (Interactive feature)
        function simulateOTP(channel) {
            const placeholder = document.getElementById('alertPlaceholder');
            const alertHtml = `
                <div class="alert alert-success alert-dismissible alert-custom fade show mb-4" role="alert">
                    <i class="bi bi-patch-check-fill me-2"></i>
                    <strong>Simulated OTP Sent!</strong> A confirmation code was dispatched to your ${channel} channel successfully. (Use any dummy 6-digit code for testing).
                    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                </div>`;
            placeholder.innerHTML = alertHtml;
            
            // Auto hide after 5 seconds
            setTimeout(() => {
                const alertEl = placeholder.querySelector('.alert');
                if (alertEl) {
                    const bsAlert = new bootstrap.Alert(alertEl);
                    bsAlert.close();
                }
            }, 5000);
        }

        // Run on page load to configure initial state
        window.addEventListener('DOMContentLoaded', () => {
            const isSame = document.getElementById('<%= rdoSameYes.ClientID %>').checked;
            togglePermanentAddress(isSame);
        });
    </script>
</body>
</html>
