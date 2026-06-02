CREATE PROCEDURE [dbo].[sp_SearchKYCRecords] 
    @Name NVARCHAR(100) = NULL, 
    @Aadhaar NVARCHAR(20) = NULL, 
    @PAN NVARCHAR(20) = NULL, 
    @Mobile NVARCHAR(15) = NULL 
AS 
BEGIN 
    SET NOCOUNT ON; 
    SELECT [Id], [FullLegalName], [AadhaarNumber], [PANNumber], [MobileNumber], [ApplicationDate], [VerificationStatus] 
    FROM [dbo].[KYCDetails] 
    WHERE (@Name IS NULL OR [FullLegalName] LIKE '%' + @Name + '%') 
      AND (@Aadhaar IS NULL OR [AadhaarNumber] = @Aadhaar) 
      AND (@PAN IS NULL OR [PANNumber] = @PAN) 
      AND (@Mobile IS NULL OR [MobileNumber] = @Mobile) 
    ORDER BY [CreatedAt] DESC; 
END
