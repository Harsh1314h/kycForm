<%@ Page Language="vb" AutoEventWireup="false" CodeFile="ManageKYC.aspx.vb" Inherits="ManageKYC" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>KYC Management Dashboard - Secure Digital Portal</title>
    
    <!-- Bootstrap 5 CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Bootstrap Icons -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css" rel="stylesheet" />
    <!-- Custom Premium Stylesheet -->
    <link href="css/style.css" rel="stylesheet" />
</head>
<body>
    <form id="adminForm" runat="server">
        <!-- Place for server-side code-behind to dynamically inject toasts -->
        <asp:Literal ID="litServerToasts" runat="server"></asp:Literal>
        <asp:Literal ID="litModalScript" runat="server"></asp:Literal>

        <!-- Floating Toast Container for Premium Notifications -->
        <div class="toast-container position-fixed top-0 end-0 p-3" style="z-index: 9999;" id="toastContainer"></div>

        <div class="kyc-container" style="max-width: 1200px;">
            
            <!-- Elegant Header Brand -->
            <div class="brand-header d-flex justify-content-between align-items-center flex-wrap gap-3">
                <div>
                    <h1>KYC Records Dashboard</h1>
                    <p>Review submitted KYC applications, verify credentials, manage status workflows, and perform core CRUD operations securely.</p>
                </div>
                <a href="Default.aspx" class="btn btn-light fw-bold px-4 py-2 d-inline-flex align-items-center gap-2 border-0 shadow-sm" style="border-radius: 12px; color: #1e3a8a;">
                    <i class="bi bi-file-earmark-plus"></i> Submit New KYC
                </a>
            </div>

            <!-- ================== ADMIN METRICS WIDGETS ================== -->
            <div class="metrics-grid">
                <div class="metric-card">
                    <div class="metric-icon total">
                        <i class="bi bi-people-fill"></i>
                    </div>
                    <div class="metric-info">
                        <div class="metric-value"><asp:Label ID="lblTotalCount" runat="server" Text="0" /></div>
                        <div class="metric-label">Total Applications</div>
                    </div>
                </div>

                <div class="metric-card">
                    <div class="metric-icon pending">
                        <i class="bi bi-hourglass-split"></i>
                    </div>
                    <div class="metric-info">
                        <div class="metric-value"><asp:Label ID="lblPendingCount" runat="server" Text="0" /></div>
                        <div class="metric-label">Pending Reviews</div>
                    </div>
                </div>

                <div class="metric-card">
                    <div class="metric-icon verified">
                        <i class="bi bi-patch-check-fill"></i>
                    </div>
                    <div class="metric-info">
                        <div class="metric-value"><asp:Label ID="lblVerifiedCount" runat="server" Text="0" /></div>
                        <div class="metric-label">Verified Accounts</div>
                    </div>
                </div>

                <div class="metric-card">
                    <div class="metric-icon rejected">
                        <i class="bi bi-x-circle-fill"></i>
                    </div>
                    <div class="metric-info">
                        <div class="metric-value"><asp:Label ID="lblRejectedCount" runat="server" Text="0" /></div>
                        <div class="metric-label">Rejected Profiles</div>
                    </div>
                </div>
            </div>

            <!-- ================== SEARCH & FILTER PANEL ================== -->
            <div class="section-card">
                <div class="section-title">
                    <i class="bi bi-search"></i>
                    <span>Search & Filter KYC Profiles</span>
                </div>
                <div class="row g-3">
                    <div class="col-md-3">
                        <label class="form-label" for="txtSearchName">Full Name</label>
                        <asp:TextBox ID="txtSearchName" runat="server" CssClass="form-control" placeholder="Search by name..."></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label" for="txtSearchAadhaar">Aadhaar Number</label>
                        <asp:TextBox ID="txtSearchAadhaar" runat="server" CssClass="form-control" placeholder="12-digit UID..." MaxLength="12"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label" for="txtSearchPAN">PAN Number</label>
                        <asp:TextBox ID="txtSearchPAN" runat="server" CssClass="form-control" placeholder="10-digit PAN..." MaxLength="10"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label" for="txtSearchMobile">Mobile Number</label>
                        <asp:TextBox ID="txtSearchMobile" runat="server" CssClass="form-control" placeholder="10-digit mobile..." MaxLength="10"></asp:TextBox>
                    </div>
                    <div class="col-12 d-flex justify-content-end gap-2">
                        <asp:LinkButton ID="btnResetSearch" runat="server" OnClick="btnResetSearch_Click" CssClass="btn btn-outline-secondary fw-semibold px-4 py-2" style="border-radius: 10px;">
                            <i class="bi bi-arrow-counterclockwise me-1"></i> Reset
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSearch" runat="server" OnClick="btnSearch_Click" CssClass="btn btn-primary fw-semibold px-4 py-2" style="border-radius: 10px; background-color: var(--primary);">
                            <i class="bi bi-funnel me-1"></i> Apply Filters
                        </asp:LinkButton>
                    </div>
                </div>
            </div>

            <!-- ================== DYNAMIC KYC DATA GRID ================== -->
            <div class="table-responsive-custom">
                <asp:Repeater ID="rptKYCList" runat="server" OnItemCommand="rptKYCList_ItemCommand">
                    <HeaderTemplate>
                        <table class="table admin-table">
                            <thead>
                                <tr>
                                    <th>Full Legal Name</th>
                                    <th>Aadhaar Number</th>
                                    <th>PAN Number</th>
                                    <th>Mobile</th>
                                    <th>Applied Date</th>
                                    <th>Status</th>
                                    <th class="text-end">Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><strong><%# Eval("FullLegalName") %></strong></td>
                            <td><code><%# MaskAadhaar(Eval("AadhaarNumber")) %></code></td>
                            <td><code><%# Eval("PANNumber") %></code></td>
                            <td><%# Eval("MobileNumber") %></td>
                            <td><%# Eval("ApplicationDate", "{0:yyyy-MM-dd}") %></td>
                            <td>
                                <span class='badge-status <%# Eval("VerificationStatus").ToString().ToLower() %>'>
                                    <i class='bi <%# GetStatusIcon(Eval("VerificationStatus")) %>'></i>
                                    <%# Eval("VerificationStatus") %>
                                </span>
                            </td>
                            <td class="text-end">
                                <div class="d-inline-flex gap-1">
                                    <asp:LinkButton ID="btnView" runat="server" CommandName="View" CommandArgument='<%# Eval("Id") %>' CssClass="btn-action view" ToolTip="View Full Profile"><i class="bi bi-eye-fill"></i></asp:LinkButton>
                                    <a href='Default.aspx?edit=<%# Eval("Id") %>' class="btn-action edit" title="Edit Profile"><i class="bi bi-pencil-square"></i></a>
                                    <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' OnClientClick="return confirm('Are you absolutely sure you want to permanently delete this KYC profile? This action is irreversible.');" CssClass="btn-action delete" ToolTip="Delete Record"><i class="bi bi-trash-fill"></i></asp:LinkButton>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                        <asp:PlaceHolder ID="pnlNoRecords" runat="server" Visible='<%# rptKYCList.Items.Count = 0 %>'>
                            <div class="text-center py-5" style="background-color: white;">
                                <i class="bi bi-clipboard2-x fs-1 text-muted opacity-50 mb-3 d-block"></i>
                                <h5 class="fw-bold text-slate-700">No KYC Records Found</h5>
                                <p class="text-muted small mb-0 px-3">No profiles matched your filtering criteria. Check inputs or add a new record.</p>
                            </div>
                        </asp:PlaceHolder>
                    </FooterTemplate>
                </asp:Repeater>
            </div>

        </div>

        <!-- ================== VIEW DETAILS & STATUS WORKFLOW MODAL ================== -->
        <asp:HiddenField ID="hdnSelectedId" runat="server" Value="" />
        
        <div class="modal fade" id="detailsModal" tabindex="-1" aria-labelledby="detailsModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content modal-content-glass">
                    <div class="modal-header modal-header-glass">
                        <h5 class="modal-title" id="detailsModalLabel">
                            <i class="bi bi-shield-lock-fill me-2 text-warning"></i>KYC Profile Information
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body modal-body-glass">
                        <asp:Panel ID="pnlModalContent" runat="server">
                            
                            <!-- Application Metadata Header -->
                            <div class="p-3 mb-4 rounded border d-flex justify-content-between align-items-center flex-wrap gap-2" style="background: rgba(37, 99, 235, 0.03);">
                                <div>
                                    <div class="text-muted small uppercase fw-bold" style="font-size: 0.7rem; letter-spacing: 0.5px;">Application Reference</div>
                                    <h5 class="fw-bold mb-0 text-dark">ID: #<asp:Label ID="lblModalId" runat="server" /></h5>
                                </div>
                                <div class="text-end">
                                    <span class="text-muted small d-block">Submitted: <strong><asp:Label ID="lblModalRegDate" runat="server" /></strong></span>
                                    <span class="text-muted small">Status: <strong><asp:Label ID="lblModalStatusText" runat="server" /></strong></span>
                                </div>
                            </div>

                            <!-- SECTION 1: BASIC ACCOUNT -->
                            <div class="detail-section-title">
                                <i class="bi bi-wallet2"></i> Account & Branch Setup
                            </div>
                            <div class="row g-3">
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Account Type</div>
                                    <div class="detail-value"><asp:Label ID="lblAccountType" runat="server" /></div>
                                </div>
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Customer Category</div>
                                    <div class="detail-value"><asp:Label ID="lblCustomerType" runat="server" /></div>
                                </div>
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Preferred Branch</div>
                                    <div class="detail-value"><asp:Label ID="lblBranch" runat="server" /></div>
                                </div>
                            </div>

                            <!-- SECTION 2: CONTACTS -->
                            <div class="detail-section-title">
                                <i class="bi bi-shield-check"></i> Verification & Contacts
                            </div>
                            <div class="row g-3">
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Email Address</div>
                                    <div class="detail-value"><asp:Label ID="lblEmail" runat="server" /></div>
                                </div>
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Primary Mobile</div>
                                    <div class="detail-value"><asp:Label ID="lblMobile" runat="server" /></div>
                                </div>
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Alternate Mobile</div>
                                    <div class="detail-value"><asp:Label ID="lblAltMobile" runat="server" /></div>
                                </div>
                            </div>

                            <!-- SECTION 3: PERSONAL -->
                            <div class="detail-section-title">
                                <i class="bi bi-person-lines-fill"></i> Personal Information
                            </div>
                            <div class="row g-3">
                                <div class="col-md-6 detail-item">
                                    <div class="detail-label">Full Legal Name</div>
                                    <div class="detail-value"><asp:Label ID="lblFullName" runat="server" /></div>
                                </div>
                                <div class="col-md-6 detail-item">
                                    <div class="detail-label">Father's Name</div>
                                    <div class="detail-value"><asp:Label ID="lblFatherName" runat="server" /></div>
                                </div>
                                <div class="col-md-6 detail-item">
                                    <div class="detail-label">Mother's Name</div>
                                    <div class="detail-value"><asp:Label ID="lblMotherName" runat="server" /></div>
                                </div>
                                <div class="col-md-6 detail-item">
                                    <div class="detail-label">Spouse/Guardian (Care Of)</div>
                                    <div class="detail-value"><asp:Label ID="lblSpouseName" runat="server" /></div>
                                </div>
                                <div class="col-md-3 detail-item">
                                    <div class="detail-label">DOB (as per UIDAI)</div>
                                    <div class="detail-value"><asp:Label ID="lblAadhaarDOB" runat="server" /></div>
                                </div>
                                <div class="col-md-3 detail-item">
                                    <div class="detail-label">Gender</div>
                                    <div class="detail-value"><asp:Label ID="lblGender" runat="server" /></div>
                                </div>
                                <div class="col-md-3 detail-item">
                                    <div class="detail-label">Marital Status</div>
                                    <div class="detail-value"><asp:Label ID="lblMarital" runat="server" /></div>
                                </div>
                                <div class="col-md-3 detail-item">
                                    <div class="detail-label">Nationality</div>
                                    <div class="detail-value"><asp:Label ID="lblNationality" runat="server" /></div>
                                </div>
                                <div class="col-md-3 detail-item">
                                    <div class="detail-label">Religion</div>
                                    <div class="detail-value"><asp:Label ID="lblReligion" runat="server" /></div>
                                </div>
                                <div class="col-md-3 detail-item">
                                    <div class="detail-label">Residential Status</div>
                                    <div class="detail-value"><asp:Label ID="lblResStatus" runat="server" /></div>
                                </div>
                                <div class="col-md-6 detail-item">
                                    <div class="detail-label">Place & Country of Birth</div>
                                    <div class="detail-value"><asp:Label ID="lblBirthPlace" runat="server" /></div>
                                </div>
                            </div>

                            <!-- SECTION 4: ADDRESS -->
                            <div class="detail-section-title">
                                <i class="bi bi-geo-alt"></i> Address Credentials
                            </div>
                            <div class="row g-3">
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Address Type</div>
                                    <div class="detail-value"><asp:Label ID="lblAddressType" runat="server" /></div>
                                </div>
                                <div class="col-md-8 detail-item">
                                    <div class="detail-label">Correspondence Address</div>
                                    <div class="detail-value"><asp:Label ID="lblCorrAddress" runat="server" /></div>
                                </div>
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Permanent Same?</div>
                                    <div class="detail-value"><asp:Label ID="lblIsSameAddress" runat="server" /></div>
                                </div>
                                <div class="col-md-8 detail-item">
                                    <div class="detail-label">Permanent Address</div>
                                    <div class="detail-value"><asp:Label ID="lblPermanentAddress" runat="server" /></div>
                                </div>
                            </div>

                            <!-- SECTION 5: OCCUPATION & INCOME -->
                            <div class="detail-section-title">
                                <i class="bi bi-briefcase"></i> Employment & Financials
                            </div>
                            <div class="row g-3">
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Occupation Type</div>
                                    <div class="detail-value"><asp:Label ID="lblOccupation" runat="server" /></div>
                                </div>
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Employer Name</div>
                                    <div class="detail-value"><asp:Label ID="lblEmployerName" runat="server" /></div>
                                </div>
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Designation</div>
                                    <div class="detail-value"><asp:Label ID="lblDesignation" runat="server" /></div>
                                </div>
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Nature of Business</div>
                                    <div class="detail-value"><asp:Label ID="lblBusinessNature" runat="server" /></div>
                                </div>
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Annual Income Range</div>
                                    <div class="detail-value"><asp:Label ID="lblIncome" runat="server" /></div>
                                </div>
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Source of Funds</div>
                                    <div class="detail-value"><asp:Label ID="lblFundsSource" runat="server" /></div>
                                </div>
                            </div>

                            <!-- SECTION 6: GOVERNMENT ID PROOFS -->
                            <div class="detail-section-title">
                                <i class="bi bi-card-text"></i> Primary Government Identifiers
                            </div>
                            <div class="row g-3">
                                <div class="col-md-6 detail-item">
                                    <div class="detail-label">Aadhaar Card UID</div>
                                    <div class="detail-value"><code><asp:Label ID="lblAadhaarNum" runat="server" /></code></div>
                                </div>
                                <div class="col-md-6 detail-item">
                                    <div class="detail-label">Aadhaar Holder Name</div>
                                    <div class="detail-value"><asp:Label ID="lblAadhaarName" runat="server" /></div>
                                </div>
                                <div class="col-md-6 detail-item">
                                    <div class="detail-label">PAN Number</div>
                                    <div class="detail-value"><code><asp:Label ID="lblPAN" runat="server" /></code></div>
                                </div>
                                <div class="col-md-6 detail-item">
                                    <div class="detail-label">PAN Holder Name</div>
                                    <div class="detail-value"><asp:Label ID="lblPANName" runat="server" /></div>
                                </div>
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">Driving Licence Number</div>
                                    <div class="detail-value"><asp:Label ID="lblDLNum" runat="server" /></div>
                                </div>
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">DL Date of Birth</div>
                                    <div class="detail-value"><asp:Label ID="lblDLDOB" runat="server" /></div>
                                </div>
                                <div class="col-md-4 detail-item">
                                    <div class="detail-label">DL Holder Name</div>
                                    <div class="detail-value"><asp:Label ID="lblDLName" runat="server" /></div>
                                </div>
                            </div>

                            <!-- SECTION 7: UPLOADED DIGITAL DOCUMENTS -->
                            <div class="detail-section-title">
                                <i class="bi bi-file-earmark-pdf"></i> Uploaded Digital Files
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="doc-card">
                                        <div class="doc-info">
                                            <i class="bi bi-person-bounding-box doc-icon"></i>
                                            <span class="doc-name">Aadhaar Card</span>
                                        </div>
                                        <asp:HyperLink ID="lnkDocAadhaar" runat="server" Target="_blank" CssClass="doc-link">
                                            <i class="bi bi-cloud-arrow-down-fill"></i> View/Download
                                        </asp:HyperLink>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="doc-card">
                                        <div class="doc-info">
                                            <i class="bi bi-credit-card-2-front doc-icon"></i>
                                            <span class="doc-name">PAN Card</span>
                                        </div>
                                        <asp:HyperLink ID="lnkDocPAN" runat="server" Target="_blank" CssClass="doc-link">
                                            <i class="bi bi-cloud-arrow-down-fill"></i> View/Download
                                        </asp:HyperLink>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="doc-card">
                                        <div class="doc-info">
                                            <i class="bi bi-feather doc-icon"></i>
                                            <span class="doc-name">Signature Scan</span>
                                        </div>
                                        <asp:HyperLink ID="lnkDocSignature" runat="server" Target="_blank" CssClass="doc-link">
                                            <i class="bi bi-cloud-arrow-down-fill"></i> View/Download
                                        </asp:HyperLink>
                                    </div>
                                </div>
                                <asp:PlaceHolder ID="pnlDocPassportDL" runat="server">
                                    <div class="col-md-6">
                                        <div class="doc-card">
                                            <div class="doc-info">
                                                <i class="bi bi-journal-bookmark-fill doc-icon"></i>
                                                <span class="doc-name">Passport / DL</span>
                                            </div>
                                            <asp:HyperLink ID="lnkDocPassportDL" runat="server" Target="_blank" CssClass="doc-link">
                                                <i class="bi bi-cloud-arrow-down-fill"></i> View/Download
                                            </asp:HyperLink>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="pnlDocAddress" runat="server">
                                    <div class="col-md-6">
                                        <div class="doc-card">
                                            <div class="doc-info">
                                                <i class="bi bi-house-door doc-icon"></i>
                                                <span class="doc-name">Address Proof</span>
                                            </div>
                                            <asp:HyperLink ID="lnkDocAddress" runat="server" Target="_blank" CssClass="doc-link">
                                                <i class="bi bi-cloud-arrow-down-fill"></i> View/Download
                                            </asp:HyperLink>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                            </div>

                            <!-- SECTION 8: VERIFICATION WORKFLOW ACTION BAR -->
                            <div class="status-action-bar">
                                <div>
                                    <div class="status-action-title">Verification Workflow Actions</div>
                                    <div class="status-action-desc">Change compliance audit status for this customer profile instantly.</div>
                                </div>
                                <div class="status-btn-group">
                                    <asp:Button ID="btnStatusPending" runat="server" OnClick="btnStatusPending_Click" Text="Mark Pending" CssClass="status-btn btn-pending" />
                                    <asp:Button ID="btnStatusVerify" runat="server" OnClick="btnStatusVerify_Click" Text="Approve Profile" CssClass="status-btn btn-verify" />
                                    <asp:Button ID="btnStatusReject" runat="server" OnClick="btnStatusReject_Click" Text="Reject Profile" CssClass="status-btn btn-reject" />
                                </div>
                            </div>

                        </asp:Panel>
                    </div>
                    <div class="modal-footer-glass d-flex justify-content-between align-items-center">
                        <span class="text-muted small">Secure Audit System Logs Active</span>
                        <button type="button" class="btn btn-secondary px-4 py-2 fw-semibold" style="border-radius: 10px;" data-bs-dismiss="modal">Close Details</button>
                    </div>
                </div>
            </div>
        </div>

    </form>

    <!-- Bootstrap 5 Bundle with Popper JS -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>

    <!-- Custom Client-Side Script to trigger modal dynamically -->
    <script type="text/javascript">
        // Show detail modal popup securely
        function showDetailsModal() {
            var myModal = new bootstrap.Modal(document.getElementById('detailsModal'));
            myModal.show();
        }

        // Elegant Toast Helper matching style.css specs
        function showToast(title, message, type) {
            const container = document.getElementById('toastContainer');
            if (!container) return;
            
            const toast = document.createElement('div');
            toast.className = `toast-custom toast-${type}`;
            
            let iconClass = 'bi-info-circle-fill';
            if (type === 'success') iconClass = 'bi-check-circle-fill';
            else if (type === 'danger') iconClass = 'bi-exclamation-triangle-fill';
            else if (type === 'warning') iconClass = 'bi-exclamation-circle-fill';
            
            toast.innerHTML = `
                <div class="toast-icon">
                    <i class="bi ${iconClass}"></i>
                </div>
                <div class="toast-content">
                    <div class="toast-title">${title}</div>
                    <div class="toast-message">${message}</div>
                </div>
                <button type="button" class="toast-close" onclick="this.parentElement.remove()">
                    <i class="bi bi-x"></i>
                </button>
            `;
            
            container.appendChild(toast);
            
            setTimeout(() => {
                toast.style.animation = 'toastSlideOut 0.3s cubic-bezier(0.16, 1, 0.3, 1) forwards';
                setTimeout(() => toast.remove(), 300);
            }, 5000);
        }
    </script>
</body>
</html>
