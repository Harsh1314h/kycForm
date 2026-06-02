CREATE PROCEDURE [dbo].[sp_GetKYCRecordById] 
    @Id INT 
AS 
BEGIN 
    SET NOCOUNT ON; 
    SELECT * FROM [dbo].[KYCDetails] WHERE [Id] = @Id; 
END
