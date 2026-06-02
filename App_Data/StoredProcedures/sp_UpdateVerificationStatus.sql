CREATE PROCEDURE [dbo].[sp_UpdateVerificationStatus] 
    @Id INT, 
    @Status NVARCHAR(20) 
AS 
BEGIN 
    SET NOCOUNT ON; 
    UPDATE [dbo].[KYCDetails] SET [VerificationStatus] = @Status WHERE [Id] = @Id; 
END
