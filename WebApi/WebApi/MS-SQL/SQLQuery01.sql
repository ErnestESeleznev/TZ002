create table ClientPayments
 (
Id bigint,
ClientId bigint,
Dt datetime2(0),
Amount money
)

insert into ClientPayments values (1,1,'2022-01-03 17:24:00',100),(2,1,'2022-01-05 17:24:14',200)
insert into ClientPayments values (3,1,'2022-01-05 18:23:34',250),(4,1,'2022-01-07 10:12:38',50)
insert into ClientPayments values (5,2,'2022-01-05 17:24:14',278),(6,2,'2022-01-10 12:39:29',300)
