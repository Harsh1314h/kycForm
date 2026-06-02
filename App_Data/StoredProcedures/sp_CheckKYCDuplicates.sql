CREATE PROCEDURE [dbo].[sp_CheckKYCDuplicates] 
    @Aadhaar NVARCHAR(20), 
    @PAN NVARCHAR(20), 
    @ExcludeId INT = 0 
AS 
BEGIN 
    SET NOCOUNT ON; 
    SELECT 
        SUM(CASE WHEN [AadhaarNumber] = @Aadhaar AND [Id] <> @ExcludeId THEN 1 ELSE 0 END) As AadhaarCount, 
        SUM(CASE WHEN [PANNumber] = @PAN AND [Id] <> @ExcludeId THEN 1 ELSE 0 END) As PANCount 
    FROM [dbo].[KYCDetails]; 
END
