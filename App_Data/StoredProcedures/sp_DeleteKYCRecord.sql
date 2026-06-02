CREATE PROCEDURE [dbo].[sp_DeleteKYCRecord] 
    @Id INT 
AS 
BEGIN 
    SET NOCOUNT ON; 
    SELECT [AadhaarCardPath], [PANCardPath], [PassportDLPath], [AddressProofPath], [SignatureScanPath] 
    FROM [dbo].[KYCDetails] 
    WHERE [Id] = @Id; 
    
    DELETE FROM [dbo].[KYCDetails] WHERE [Id] = @Id; 
END
