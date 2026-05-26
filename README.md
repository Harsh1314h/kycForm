# Secure Digital KYC Portal (Internship Project)

A robust, enterprise-grade digital KYC (Know Your Customer) Verification Form designed for financial and banking applications. Built using a modern, premium responsive interface and a classic backend compiler structure.

---

## 📅 Project Implementation Roadmap (6-Day Plan)

- [x] **Day 1: Project Setup + Basic UI (49% Fields - 4 Sections)**
- [ ] **Day 2: Complete Form + Frontend Valdiation**
- [ ] **Day 3: Database Design (SSMS) + Connection**
- [ ] **Day 4: Save Functionality (CREATE/INSERT)**
- [ ] **Day 5: Search + Update + Delete (Full CRUD)**
- [ ] **Day 6: Testing, Spacing, and Final Polish**

---

## 🛠️ Technology Stack (Day 1 Status)
- **Frontend Core**: HTML5 & Vanilla CSS3 (Custom aesthetics overriding Bootstrap)
- **Styling Framework**: Bootstrap 5 (Responsive flex grids, tables, and icons)
- **Backend Platform**: ASP.NET Web Forms
- **Language**: VB.NET (Visual Basic .NET)
- **Development Tooling**: Visual Studio / MSBuild / VB.NET Compiler v14 (`vbc.exe`)
- **Version Control**: Git

---

## 📁 Project Directory Structure
```text
c:\kycform\
│
├── .gitignore                   # Visual Studio & Windows Git ignore rules
├── KYCForm.sln                  # Visual Studio Solution File
├── KYCForm.vbproj               # MSBuild Web Application Project File
├── Web.config                   # ASP.NET Application configuration (targetFramework v4.0)
│
├── Default.aspx                 # Core UI with 4 sections & 25 interactive fields
├── Default.aspx.vb              # VB.NET Code-Behind containing page load logic and control declarations
│
├── My Project/
│   └── AssemblyInfo.vb          # Assembly metadata details
│
├── bin/
│   └── KYCForm.dll              # Compiled VB.NET Assembly binary
│
└── css/
    └── style.css                # Premium Glassmorphism & Plus Jakarta Sans styles
```

---

## ✨ Day 1 - Visual & UI Highlights
1. **Interactive Layout**: Includes 4 core sections from the specification:
   - **Section 1: Basic Account Info** (Account Type, Customer Type, Branch, Date)
   - **Section 2: Contact & Verification** (Email, Email OTP, Mobile Number, Alternate Mobile, Aadhaar Mobile OTP)
   - **Section 3: Aadhaar Details** (Aadhaar Number, Aadhaar OTP, Aadhaar Name, Aadhaar DOB, Gender)
   - **Section 5: Address Details** (Street, Locality, Town, P.O., District, State, Country, Pincode, Address Type, Same-as-Permanent toggle, Permanent Address)
2. **Premium Look & Feel**: Beautiful typography using **Plus Jakarta Sans**, deep slate headers, floating glass-like section panels, and glowing outlines on input fields.
3. **Simulated OTP Dispatches**: Clicking "Send OTP" alerts the user with a simulated notification toast without reloading the page.
4. **Smart Address Copy & Hide**: Pure client-side JavaScript handles the toggle of different permanent addresses dynamically to minimize form clutter.

---

## 💻 How to Compile and Run
### 1. Direct VB.NET Code Compilation
Since standard MSBuild Targeting Packs can sometimes be missing on lightweight Build Tool installations, the project can be successfully compiled using the raw **VB.NET Compiler** (`vbc.exe`) installed on any Windows machine:

```powershell
# Compiles the VB.NET code-behind directly into standard Web DLL
& "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\vbc.exe" /target:library /out:bin\KYCForm.dll /r:System.dll,System.Web.dll,System.Data.dll,System.Xml.dll,System.Core.dll Default.aspx.vb "My Project\AssemblyInfo.vb" /optionexplicit+ /optionstrict- /optioncompare:binary /imports:Microsoft.VisualBasic,System,System.Collections,System.Collections.Generic,System.Data,System.Diagnostics,System.Linq,System.Web,System.Web.UI,System.Web.UI.WebControls
```

### 2. Loading into Visual Studio
Simply double-click `KYCForm.sln` to open the complete, pre-configured solution in Visual Studio.
- Press `F5` to run via the IIS Express integrated web server.
