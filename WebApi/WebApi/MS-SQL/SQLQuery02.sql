USE [TestDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create FUNCTION [dbo].[f_daily_amount]
(	
    @ClientId int,
    @Sd date,
    @Ed date
)
RETURNS TABLE 
AS
RETURN 
(
WITH Dates AS (
    SELECT @Sd AS Dt
    UNION ALL
    SELECT DATEADD(DAY, 1, Dt)
    FROM Dates
    WHERE Dt < @Ed
)
select d.Dt, sum(isnull(Amount,0)) [Ñóììà] from Dates d
    left join ClientPayments cp on cast(cp.Dt as date) = d.Dt and ClientId = @ClientId
	group by d.Dt

)
