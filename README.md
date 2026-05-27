# Secure Digital KYC Portal

A premium, enterprise-grade digital KYC (Know Your Customer) Verification Portal designed for secure, digital-first banking and financial applications. The application delivers a state-of-the-art visual experience (glassmorphism, tailored HSL color schemes, and modern typography) backed by a robust and clean ASP.NET compiler layout.

---

## 🌟 Key Features & Form Sections

The portal implements the complete multi-page standard KYC specification within a single, beautifully responsive container:

### 1. Basic Account Information (Section 1)
* Selectable Account Types (Savings, Current, Fixed, etc.) and Customer Types (Individual / Non-Individual).
* Preferred Bank selector pre-populated with major Indian Banks (SBI, HDFC, ICICI, Axis, PNB, BOB, Canara, Union).
* Automatic Application Date binding.

### 2. Contact & OTP Verification (Section 2)
* Inline input layout containing email and mobile entries.
* Interactive **Get OTP** triggers next to inputs displaying modern notification dispatches.
* Integrated inputs for email OTP and Aadhaar-registered mobile OTP verification.

### 3. Aadhaar (UIDAI) Details (Section 3)
* Dedicated Aadhaar number validation with matching verification OTP fields.
* Synchronized input for Aadhaar Legal Name, Date of Birth, and Gender.

### 4. Personal Information (Section 4)
* Full legal identity records: Full Name, Father's Name, Mother's Name, and Spouse/Guardian details.
* Standard dropdown matrices for Marital Status, Nationality, Religion, Residential Status (Resident, NRI, OCI, PIO), and Birthplace.

### 5. Address Details (Current & Permanent) (Section 5)
* Comprehensive correspondence address details including Street, Locality, P.O., City, State (with all 36 Indian States/UTs fully mapped), and Pincode.
* Smart **Permanent Address Toggle**: Select "Yes" to automatically hide the fields, or "No" to expand a custom, animated multi-line textbox for a separate permanent address.

### 6. Employment & Financials (Section 6)
* Occupation selectors (Salaried, Business, Retired, Student, Housewife, Other).
* Business/Employer tags, industry designations, annual income brackets, and source of funds radio choices.

### 7. Banking & ID Details (Section 7)
* Secure government ID details mapping.
* Permanent Account Number (PAN) capture and optional Driving Licence registration.

### 8. Premium Document Uploads (Section 8)
* Visual upload zones representing drag-and-drop styled boxes (dash-bordered containers).
* Live file validation: Only accepts `.pdf`, `.jpg`, or `.jpeg` under `5MB` size. Displays warnings immediately upon selection.
* Micro-visual file previews showing filename, size, and document badges on successful upload, alongside a delete icon.

---

## 🛡️ Interactive Client-Side Validation Engine

The frontend incorporates a professional, real-time validation network built in JavaScript:
* **Immediate Character Filtering**: Prevents illegal character entries in real-time on numeric inputs (Pincode, Aadhaar, Mobile, OTP).
* **Automatic Formatting**: Automatically forces text capitalization on government IDs like PAN.
* **Inline States**: Triggers visual checkmarks (emerald borders) or validation errors (crimson borders) instantly on field `blur` (lost focus).
* **Floating Toasts**: Dispatches SaaS-dashboard style sliding notification alerts at the top-right of the viewport for error reviews or successful saves.
* **Auto-Focus Scrolling**: If validations fail on "Save", the page details the warning, shows a toast, and scrolls smoothly to focus the first invalid control.

---

## 📁 Project Directory Structure

The project has a clean layout ignoring compiled assets and local development caches:

```text
c:\kycform\
│
├── .gitignore                   # Visual Studio & git exclude rules
├── KYCForm.sln                  # Visual Studio Project Solution
├── KYCForm.vbproj               # MSBuild Web Application Project File
├── Web.config                   # ASP.NET Application configuration (targetFramework v4.8)
│
├── Default.aspx                 # Main UI markup containing all 8 sections and validation scripts
├── Default.aspx.vb              # VB.NET Code-behind using dynamic control resolution
│
└── css/
    └── style.css                # Premium custom styling sheet (typography, glassmorphism, animations)
```

---

## 🚀 How to Run Locally

### Visual Studio
1. Open the folder in Visual Studio or double-click the solution file `KYCForm.sln`.
2. Ensure you have the **ASP.NET and Web Development** workload installed.
3. Select `Default.aspx` and click the green **Play (IIS Express)** button to load it in your browser.

*(Note: The page is designed using dynamic compilation. You will encounter zero designer file warnings or assembly namespace clashing on build.)*
