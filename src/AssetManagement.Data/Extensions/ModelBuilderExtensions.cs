using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void SeedUsers(this ModelBuilder modelBuilder)
        {
            var fixedCreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = new Guid("c87d8f20-c381-4970-b367-09f2cf95d655"),
                    StaffCode = "SD0001",
                    FirstName = "Nghia",
                    LastName = "Dinh",
                    Username = "nghiadinh",
                    Password = "SXiTq48SSiVAhIjU3TW8PHbHJ2K2geU8aoV10m9y643uUGO1pI/m7s0d5pciA0bd",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1990, 1, 1),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 1, 10)),
                    Type = UserTypeEnum.Admin,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("1f2b0a56-3c8e-4771-b227-69e2cd5e41a3"),
                    StaffCode = "SD0002",
                    FirstName = "Minh",
                    LastName = "Nguyen",
                    Username = "minhnguyen",
                    Password = "SXiTq48SSiVAhIjU3TW8PHbHJ2K2geU8aoV10m9y643uUGO1pI/m7s0d5pciA0bd",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1992, 5, 15),
                    JoinedDate = new DateTimeOffset(new DateTime(2021, 3, 20)),
                    Type = UserTypeEnum.Admin,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },

            #region Ho Chi Minh Users
                new User
                {
                    Id = new Guid("2b6d4d81-7305-4cd3-9104-760aa2d9b80a"),
                    StaffCode = "SD0003",
                    FirstName = "Linh",
                    LastName = "Tran",
                    Username = "linhtran",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1994, 9, 8),
                    JoinedDate = new DateTimeOffset(new DateTime(2022, 2, 15)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Female,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("3c57b69c-d9a0-4678-b94f-ad4533be6e81"),
                    StaffCode = "SD0004",
                    FirstName = "Tuan",
                    LastName = "Pham",
                    Username = "tuanpham",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1991, 11, 23),
                    JoinedDate = new DateTimeOffset(new DateTime(2021, 7, 5)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HN,
                    Gender = GenderEnum.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("4f38d797-4168-4c55-8884-76f1f2d3f79b"),
                    StaffCode = "SD0005",
                    FirstName = "Hoa",
                    LastName = "Le",
                    Username = "hoale",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1993, 3, 18),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 9, 12)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Female,
                    IsActive = false,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("6abd3850-93f5-458a-94ed-4ccd3cb903df"),
                    StaffCode = "SD0006",
                    FirstName = "An",
                    LastName = "Nguyen",
                    Username = "annguyen",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1990, 5, 10),
                    JoinedDate = new DateTimeOffset(new DateTime(2021, 1, 10)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("d445a21d-93a2-4d4e-b1e4-421717680965"),
                    StaffCode = "SD0007",
                    FirstName = "Minh",
                    LastName = "Tran",
                    Username = "minhtran",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1992, 8, 15),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 3, 1)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("2b1dc1f3-945e-444d-b68d-52e59d0e699c"),
                    StaffCode = "SD0008",
                    FirstName = "Linh",
                    LastName = "Pham",
                    Username = "linhpham",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1994, 4, 20),
                    JoinedDate = new DateTimeOffset(new DateTime(2022, 5, 5)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Female,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("88108306-c04f-4ab4-be0b-2ed7a813043d"),
                    StaffCode = "SD0009",
                    FirstName = "Tuan",
                    LastName = "Le",
                    Username = "tuanle",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1989, 9, 30),
                    JoinedDate = new DateTimeOffset(new DateTime(2019, 12, 15)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("2246b919-73f8-44f2-bcc2-80149e40a796"),
                    StaffCode = "SD0010",
                    FirstName = "Trang",
                    LastName = "Do",
                    Username = "trangdo",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1995, 12, 5),
                    JoinedDate = new DateTimeOffset(new DateTime(2021, 6, 18)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Female,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("91a21848-2258-4cff-b6a8-31e75ec69ed0"),
                    StaffCode = "SD0011",
                    FirstName = "Khanh",
                    LastName = "Vo",
                    Username = "khanhvo",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1991, 11, 23),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 8, 22)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("dbf2b2c5-52a7-46ea-808d-e29538529105"),
                    StaffCode = "SD0012",
                    FirstName = "Thao",
                    LastName = "Nguyen",
                    Username = "thaonguyen",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1996, 2, 14),
                    JoinedDate = new DateTimeOffset(new DateTime(2023, 2, 1)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Female,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("00b68283-d752-4fff-be4c-a0031a787876"),
                    StaffCode = "SD0013",
                    FirstName = "Hieu",
                    LastName = "Pham",
                    Username = "hieupham",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1993, 7, 17),
                    JoinedDate = new DateTimeOffset(new DateTime(2018, 11, 20)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                // Inactive users
                new User
                {
                    Id = new Guid("f74f2293-213d-41f3-b146-7dd4acb5499c"),
                    StaffCode = "SD0014",
                    FirstName = "Lan",
                    LastName = "Nguyen",
                    Username = "lannguyen",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1988, 6, 9),
                    JoinedDate = new DateTimeOffset(new DateTime(2017, 4, 10)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Female,
                    IsActive = false,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("edaa920f-f98e-40a8-88ab-56ffe9e99093"),
                    StaffCode = "SD0015",
                    FirstName = "Duy",
                    LastName = "Hoang",
                    Username = "duyhoang",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1990, 10, 2),
                    JoinedDate = new DateTimeOffset(new DateTime(2016, 7, 25)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Male,
                    IsActive = false,
                    CreatedDate = fixedCreatedDate
                },
            #endregion

            #region Da Nang Users
                new User
                {
                    Id = new Guid("edf1b40d-c5e2-410b-859e-ec29a5d95292"),
                    StaffCode = "SD0016",
                    FirstName = "Binh",
                    LastName = "Tran",
                    Username = "binhtran",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1991, 4, 12),
                    JoinedDate = new DateTimeOffset(new DateTime(2022, 3, 10)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("024e81ce-ef22-458b-bf29-e6b3bfec2fb8"),
                    StaffCode = "SD0017",
                    FirstName = "Ha",
                    LastName = "Nguyen",
                    Username = "hanguyen",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1993, 2, 8),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 11, 5)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Female,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("110fc2ec-7d7d-4955-9505-0d962e0c1deb"),
                    StaffCode = "SD0018",
                    FirstName = "Minh",
                    LastName = "Le",
                    Username = "minhle",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1990, 8, 23),
                    JoinedDate = new DateTimeOffset(new DateTime(2021, 7, 3)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("56d73f5a-91be-4552-b356-dab3367315f1"),
                    StaffCode = "SD0019",
                    FirstName = "Thao",
                    LastName = "Phan",
                    Username = "thaophan",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1992, 9, 15),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 5, 1)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Female,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("480826b4-b3f7-4838-b830-1a673a508fdb"),
                    StaffCode = "SD0020",
                    FirstName = "Long",
                    LastName = "Dang",
                    Username = "longdang",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1994, 11, 2),
                    JoinedDate = new DateTimeOffset(new DateTime(2023, 2, 18)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("4f8327a0-c573-4e28-bdce-c05b28aaab07"),
                    StaffCode = "SD0021",
                    FirstName = "Quynh",
                    LastName = "Pham",
                    Username = "quynhpham",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1991, 6, 20),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 1, 9)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Female,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("4a7ee807-4881-4b19-9fa5-4caefa6ef653"),
                    StaffCode = "SD0022",
                    FirstName = "Dung",
                    LastName = "Ly",
                    Username = "dungly",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1989, 3, 14),
                    JoinedDate = new DateTimeOffset(new DateTime(2022, 6, 30)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("aae93aa5-632c-4101-ac5a-ae3bf7c1b031"),
                    StaffCode = "SD0023",
                    FirstName = "Huong",
                    LastName = "Do",
                    Username = "huongdo",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1995, 7, 9),
                    JoinedDate = new DateTimeOffset(new DateTime(2023, 9, 5)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Female,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("d32543f8-1346-4c0d-af20-17edf8149d75"),
                    StaffCode = "SD0024",
                    FirstName = "Phong",
                    LastName = "Hoang",
                    Username = "phonghoang",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1990, 10, 11),
                    JoinedDate = new DateTimeOffset(new DateTime(2021, 8, 12)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },

                // Inactive users
                new User
                {
                    Id = new Guid("43fcafa3-be42-4100-91d2-6390b1a81780"),
                    StaffCode = "SD0025",
                    FirstName = "Tram",
                    LastName = "Ngo",
                    Username = "tramngo",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1991, 1, 29),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 10, 20)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Female,
                    IsActive = false,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("9b05c83e-410f-4ca6-aa7b-98c1bc63b6f2"),
                    StaffCode = "SD0026",
                    FirstName = "Khoi",
                    LastName = "Phan",
                    Username = "khoiphan",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1992, 5, 22),
                    JoinedDate = new DateTimeOffset(new DateTime(2019, 6, 30)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Male,
                    IsActive = false,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("43ea65e7-4878-45dc-b730-47c3b2dba729"),
                    StaffCode = "SD0027",
                    FirstName = "Yen",
                    LastName = "Lam",
                    Username = "yenlam",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1993, 9, 17),
                    JoinedDate = new DateTimeOffset(new DateTime(2022, 1, 4)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Female,
                    IsActive = false,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("6a527ed0-cf45-4076-b930-d0183e274cc4"),
                    StaffCode = "SD0028",
                    FirstName = "Vinh",
                    LastName = "Dinh",
                    Username = "vinhdinh",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1990, 4, 30),
                    JoinedDate = new DateTimeOffset(new DateTime(2018, 5, 5)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Male,
                    IsActive = false,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("18c5c4c2-e04a-4e9f-8c83-370e5a03ca1c"),
                    StaffCode = "SD0029",
                    FirstName = "Lan",
                    LastName = "Bui",
                    Username = "lanbui",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1995, 6, 19),
                    JoinedDate = new DateTimeOffset(new DateTime(2023, 3, 8)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Female,
                    IsActive = false,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("a2786e5b-9b10-4c24-92a0-915d686d1086"),
                    StaffCode = "SD0030",
                    FirstName = "Tien",
                    LastName = "Vu",
                    Username = "tienvu",
                    Password = "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1988, 12, 27),
                    JoinedDate = new DateTimeOffset(new DateTime(2016, 12, 15)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Male,
                    IsActive = false,
                    CreatedDate = fixedCreatedDate
                }
            #endregion
            );
        }
    }
}
