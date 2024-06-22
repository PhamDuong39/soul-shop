create table Catalog_Brand
(
    Id          int auto_increment constraint `PRIMARY`
			primary key,
    Name        varchar(450) charset utf8mb4 not null,
    Slug        varchar(450) charset utf8mb4 not null,
    Description longtext charset utf8mb4     null,
    IsPublished tinyint(1)                   not null,
    IsDeleted   tinyint(1)                   not null,
    CreatedOn   datetime(6)                  not null,
    UpdatedOn   datetime(6)                  not null
)
    engine = InnoDB;

create table Catalog_ProductAttributeGroup
(
    Id        int auto_increment constraint `PRIMARY`
			primary key,
    Name      varchar(450) charset utf8mb4 not null,
    IsDeleted tinyint(1)                   not null,
    CreatedOn datetime(6)                  not null,
    UpdatedOn datetime(6)                  not null
)
    engine = InnoDB;

create table Catalog_ProductAttribute
(
    Id        int auto_increment constraint `PRIMARY`
			primary key,
    Name      varchar(450) charset utf8mb4 not null,
    GroupId   int                          not null,
    IsDeleted tinyint(1)                   not null,
    CreatedOn datetime(6)                  not null,
    UpdatedOn datetime(6)                  not null,
    constraint `FK_Catalog_ProductAttribute_Catalog_ProductAttributeGroup_Group~`
        foreign key (GroupId) references Catalog_ProductAttributeGroup (Id)
)
    engine = InnoDB;

create index IX_Catalog_ProductAttribute_GroupId
    on Catalog_ProductAttribute (GroupId);

create table Catalog_ProductAttributeData
(
    Id          int auto_increment constraint `PRIMARY`
			primary key,
    AttributeId int                          not null,
    Value       varchar(450) charset utf8mb4 not null,
    Description longtext charset utf8mb4     null,
    IsPublished tinyint(1)                   not null,
    IsDeleted   tinyint(1)                   not null,
    CreatedOn   datetime(6)                  not null,
    UpdatedOn   datetime(6)                  not null,
    constraint `FK_Catalog_ProductAttributeData_Catalog_ProductAttribute_Attrib~`
        foreign key (AttributeId) references Catalog_ProductAttribute (Id)
)
    engine = InnoDB;

create index IX_Catalog_ProductAttributeData_AttributeId
    on Catalog_ProductAttributeData (AttributeId);

create table Catalog_ProductAttributeTemplate
(
    Id        int auto_increment constraint `PRIMARY`
			primary key,
    Name      varchar(450) charset utf8mb4 not null,
    IsDeleted tinyint(1)                   not null,
    CreatedOn datetime(6)                  not null,
    UpdatedOn datetime(6)                  not null
)
    engine = InnoDB;

create table Catalog_ProductAttributeTemplateRelation
(
    Id          int auto_increment constraint `PRIMARY`
			primary key,
    TemplateId  int         not null,
    AttributeId int         not null,
    IsDeleted   tinyint(1)  not null,
    CreatedOn   datetime(6) not null,
    UpdatedOn   datetime(6) not null,
    constraint `FK_Catalog_ProductAttributeTemplateRelation_Catalog_ProductAttr~`
        foreign key (AttributeId) references Catalog_ProductAttribute (Id)
            on delete cascade,
    constraint `FK_Catalog_ProductAttributeTemplateRelation_Catalog_ProductAtt~1`
        foreign key (TemplateId) references Catalog_ProductAttributeTemplate (Id)
            on delete cascade
)
    engine = InnoDB;

create index IX_Catalog_ProductAttributeTemplateRelation_AttributeId
    on Catalog_ProductAttributeTemplateRelation (AttributeId);

create index IX_Catalog_ProductAttributeTemplateRelation_TemplateId
    on Catalog_ProductAttributeTemplateRelation (TemplateId);

create table Catalog_ProductOption
(
    Id          int auto_increment constraint `PRIMARY`
			primary key,
    Name        varchar(450) charset utf8mb4 not null,
    DisplayType int                          not null,
    IsDeleted   tinyint(1)                   not null,
    CreatedOn   datetime(6)                  not null,
    UpdatedOn   datetime(6)                  not null
)
    engine = InnoDB;

create table Catalog_ProductOptionData
(
    Id          int auto_increment constraint `PRIMARY`
			primary key,
    OptionId    int                          not null,
    Value       varchar(450) charset utf8mb4 not null,
    Display     varchar(450) charset utf8mb4 null,
    Description longtext charset utf8mb4     null,
    IsPublished tinyint(1)                   not null,
    IsDeleted   tinyint(1)                   not null,
    CreatedOn   datetime(6)                  not null,
    UpdatedOn   datetime(6)                  not null,
    constraint FK_Catalog_ProductOptionData_Catalog_ProductOption_OptionId
        foreign key (OptionId) references Catalog_ProductOption (Id)
)
    engine = InnoDB;

create index IX_Catalog_ProductOptionData_OptionId
    on Catalog_ProductOptionData (OptionId);

create table Catalog_Unit
(
    Id        int auto_increment constraint `PRIMARY`
			primary key,
    Name      longtext charset utf8mb4 null,
    IsDeleted tinyint(1)               not null,
    CreatedOn datetime(6)              not null,
    UpdatedOn datetime(6)              not null
)
    engine = InnoDB;

create table Core_AppSetting
(
    Id                           varchar(255) charset utf8mb4 not null constraint `PRIMARY`
			primary key,
    Module                       varchar(450) charset utf8mb4 null,
    FormatType                   int                          not null,
    Type                         varchar(450) charset utf8mb4 null,
    Value                        longtext charset utf8mb4     null,
    IsVisibleInCommonSettingPage tinyint(1)                   not null,
    Note                         varchar(450) charset utf8mb4 null
)
    engine = InnoDB;

create table Core_Country
(
    Id                 int auto_increment constraint `PRIMARY`
			primary key,
    Name               varchar(450) charset utf8mb4 not null,
    TwoLetterIsoCode   varchar(450) charset utf8mb4 not null,
    ThreeLetterIsoCode longtext charset utf8mb4     not null,
    NumericIsoCode     int                          not null,
    IsBillingEnabled   tinyint(1)                   not null,
    IsShippingEnabled  tinyint(1)                   not null,
    IsCityEnabled      tinyint(1)                   not null,
    IsDistrictEnabled  tinyint(1)                   not null,
    DisplayOrder       int                          not null,
    IsPublished        tinyint(1)                   not null,
    IsDeleted          tinyint(1)                   not null,
    CreatedOn          datetime(6)                  not null,
    UpdatedOn          datetime(6)                  not null
)
    engine = InnoDB;

create table Core_EmailSend
(
    Id        int auto_increment constraint `PRIMARY`
			primary key,
    `From`    longtext charset utf8mb4     not null,
    `To`      longtext charset utf8mb4     null,
    Cc        longtext charset utf8mb4     null,
    Bcc       longtext charset utf8mb4     null,
    Subject   longtext charset utf8mb4     null,
    Body      longtext charset utf8mb4     null,
    IsHtml    tinyint(1)                   not null,
    OutId     varchar(450) charset utf8mb4 null,
    ReceiptId varchar(450) charset utf8mb4 null,
    IsSucceed tinyint(1)                   not null,
    Message   longtext charset utf8mb4     null,
    IsDeleted tinyint(1)                   not null,
    CreatedOn datetime(6)                  not null,
    UpdatedOn datetime(6)                  not null
)
    engine = InnoDB;

create table Core_EntityType
(
    Id         int auto_increment constraint `PRIMARY`
			primary key,
    Name       varchar(450) charset utf8mb4 not null,
    IsMenuable tinyint(1)                   not null,
    Module     varchar(450) charset utf8mb4 not null
)
    engine = InnoDB;

create table Core_Entity
(
    Id           int auto_increment constraint `PRIMARY`
			primary key,
    Name         varchar(450) charset utf8mb4 not null,
    Slug         varchar(450) charset utf8mb4 not null,
    EntityId     int                          not null,
    EntityTypeId int                          not null,
    IsDeleted    tinyint(1)                   not null,
    CreatedOn    datetime(6)                  not null,
    UpdatedOn    datetime(6)                  not null,
    constraint FK_Core_Entity_Core_EntityType_EntityTypeId
        foreign key (EntityTypeId) references Core_EntityType (Id)
)
    engine = InnoDB;

create index IX_Core_Entity_EntityTypeId
    on Core_Entity (EntityTypeId);

create table Core_Media
(
    Id        int auto_increment constraint `PRIMARY`
			primary key,
    Caption   varchar(450) charset utf8mb4 null,
    FileSize  int                          not null,
    FileName  varchar(450) charset utf8mb4 not null,
    Url       longtext charset utf8mb4     null,
    Path      longtext charset utf8mb4     null,
    Host      varchar(450) charset utf8mb4 null,
    Hash      varchar(450) charset utf8mb4 null,
    Md5       varchar(32) charset utf8mb4  null,
    MediaType int                          not null,
    CreatedOn datetime(6)                  not null,
    UpdatedOn datetime(6)                  not null
)
    engine = InnoDB;

create table Catalog_Category
(
    Id              int auto_increment constraint `PRIMARY`
			primary key,
    Name            varchar(450) charset utf8mb4 not null,
    Slug            varchar(450) charset utf8mb4 not null,
    MetaTitle       longtext charset utf8mb4     null,
    MetaKeywords    longtext charset utf8mb4     null,
    MetaDescription longtext charset utf8mb4     null,
    Description     longtext charset utf8mb4     null,
    DisplayOrder    int                          not null,
    IsPublished     tinyint(1)                   not null,
    IncludeInMenu   tinyint(1)                   not null,
    ParentId        int                          null,
    MediaId         int                          null,
    IsDeleted       tinyint(1)                   not null,
    CreatedOn       datetime(6)                  not null,
    UpdatedOn       datetime(6)                  not null,
    constraint FK_Catalog_Category_Catalog_Category_ParentId
        foreign key (ParentId) references Catalog_Category (Id),
    constraint FK_Catalog_Category_Core_Media_MediaId
        foreign key (MediaId) references Core_Media (Id)
)
    engine = InnoDB;

create index IX_Catalog_Category_MediaId
    on Catalog_Category (MediaId);

create index IX_Catalog_Category_ParentId
    on Catalog_Category (ParentId);

create table Core_Role
(
    Id               int auto_increment constraint `PRIMARY`
			primary key,
    Name             varchar(256) charset utf8mb4 null,
    NormalizedName   varchar(256) charset utf8mb4 null,
    ConcurrencyStamp longtext charset utf8mb4     null,
    constraint RoleNameIndex
        unique (NormalizedName)
)
    engine = InnoDB;

create table Core_RoleClaim
(
    Id         int auto_increment constraint `PRIMARY`
			primary key,
    RoleId     int                      not null,
    ClaimType  longtext charset utf8mb4 null,
    ClaimValue longtext charset utf8mb4 null,
    constraint FK_Core_RoleClaim_Core_Role_RoleId
        foreign key (RoleId) references Core_Role (Id)
            on delete cascade
)
    engine = InnoDB;

create index IX_Core_RoleClaim_RoleId
    on Core_RoleClaim (RoleId);

create table Core_SmsSend
(
    Id            int auto_increment constraint `PRIMARY`
			primary key,
    PhoneNumber   varchar(450) charset utf8mb4 not null,
    Value         varchar(450) charset utf8mb4 null,
    SignName      varchar(450) charset utf8mb4 null,
    TemplateType  int                          null,
    TemplateCode  varchar(450) charset utf8mb4 null,
    TemplateParam longtext charset utf8mb4     null,
    OutId         varchar(450) charset utf8mb4 null,
    ReceiptId     varchar(450) charset utf8mb4 null,
    IsUsed        tinyint(1)                   not null,
    IsSucceed     tinyint(1)                   not null,
    IsTest        tinyint(1)                   not null,
    Message       longtext charset utf8mb4     null,
    IsDeleted     tinyint(1)                   not null,
    CreatedOn     datetime(6)                  not null,
    UpdatedOn     datetime(6)                  not null
)
    engine = InnoDB;

create index IX_Core_SmsSend_IsSucceed
    on Core_SmsSend (IsSucceed);

create index IX_Core_SmsSend_IsUsed
    on Core_SmsSend (IsUsed);

create index IX_Core_SmsSend_PhoneNumber
    on Core_SmsSend (PhoneNumber);

create table Core_StateOrProvince
(
    Id           int auto_increment constraint `PRIMARY`
			primary key,
    ParentId     int                          null,
    CountryId    int                          not null,
    Code         varchar(450) charset utf8mb4 null,
    Name         varchar(450) charset utf8mb4 not null,
    Level        int                          not null,
    DisplayOrder int                          not null,
    IsPublished  tinyint(1)                   not null,
    IsDeleted    tinyint(1)                   not null,
    CreatedOn    datetime(6)                  not null,
    UpdatedOn    datetime(6)                  not null,
    constraint FK_Core_StateOrProvince_Core_Country_CountryId
        foreign key (CountryId) references Core_Country (Id),
    constraint FK_Core_StateOrProvince_Core_StateOrProvince_ParentId
        foreign key (ParentId) references Core_StateOrProvince (Id)
)
    engine = InnoDB;

create table Core_Address
(
    Id                int auto_increment constraint `PRIMARY`
			primary key,
    ContactName       longtext charset utf8mb4 null,
    Phone             longtext charset utf8mb4 null,
    AddressLine1      longtext charset utf8mb4 null,
    AddressLine2      longtext charset utf8mb4 null,
    City              longtext charset utf8mb4 null,
    ZipCode           longtext charset utf8mb4 null,
    Email             longtext charset utf8mb4 null,
    Company           longtext charset utf8mb4 null,
    StateOrProvinceId int                      not null,
    CountryId         int                      not null,
    IsDeleted         tinyint(1)               not null,
    CreatedOn         datetime(6)              not null,
    UpdatedOn         datetime(6)              not null,
    constraint FK_Core_Address_Core_Country_CountryId
        foreign key (CountryId) references Core_Country (Id),
    constraint FK_Core_Address_Core_StateOrProvince_StateOrProvinceId
        foreign key (StateOrProvinceId) references Core_StateOrProvince (Id)
)
    engine = InnoDB;

create index IX_Core_Address_CountryId
    on Core_Address (CountryId);

create index IX_Core_Address_StateOrProvinceId
    on Core_Address (StateOrProvinceId);

create index IX_Core_StateOrProvince_CountryId
    on Core_StateOrProvince (CountryId);

create index IX_Core_StateOrProvince_ParentId
    on Core_StateOrProvince (ParentId);

create table Core_User
(
    Id                       int auto_increment constraint `PRIMARY`
			primary key,
    UserName                 varchar(256) charset utf8mb4 null,
    NormalizedUserName       varchar(256) charset utf8mb4 null,
    Email                    varchar(256) charset utf8mb4 null,
    NormalizedEmail          varchar(256) charset utf8mb4 null,
    EmailConfirmed           tinyint(1)                   not null,
    PasswordHash             longtext charset utf8mb4     null,
    SecurityStamp            longtext charset utf8mb4     null,
    ConcurrencyStamp         longtext charset utf8mb4     null,
    PhoneNumber              varchar(255) charset utf8mb4 null,
    PhoneNumberConfirmed     tinyint(1)                   not null,
    TwoFactorEnabled         tinyint(1)                   not null,
    LockoutEnd               datetime(6)                  null,
    LockoutEnabled           tinyint(1)                   not null,
    AccessFailedCount        int                          not null,
    UserGuid                 char(36)                     not null,
    FullName                 varchar(450) charset utf8mb4 not null,
    DefaultShippingAddressId int                          null,
    DefaultBillingAddressId  int                          null,
    RefreshTokenHash         longtext charset utf8mb4     null,
    Culture                  longtext charset utf8mb4     null,
    ExtensionData            longtext charset utf8mb4     null,
    IsActive                 tinyint(1)                   not null,
    LastIpAddress            varchar(450) charset utf8mb4 null,
    LastLoginOn              datetime(6)                  null,
    LastActivityOn           datetime(6)                  null,
    AvatarUrl                varchar(450) charset utf8mb4 null,
    AvatarId                 int                          null,
    AdminRemark              varchar(450) charset utf8mb4 null,
    IsDeleted                tinyint(1)                   not null,
    CreatedOn                datetime(6)                  not null,
    UpdatedOn                datetime(6)                  not null,
    constraint IX_Core_User_Email
        unique (Email),
    constraint IX_Core_User_PhoneNumber
        unique (PhoneNumber),
    constraint IX_Core_User_UserName
        unique (UserName),
    constraint UserNameIndex
        unique (NormalizedUserName),
    constraint FK_Core_User_Core_Media_AvatarId
        foreign key (AvatarId) references Core_Media (Id)
)
    engine = InnoDB;

create index EmailIndex
    on Core_User (NormalizedEmail);

create index IX_Core_User_AvatarId
    on Core_User (AvatarId);

create index IX_Core_User_DefaultBillingAddressId
    on Core_User (DefaultBillingAddressId);

create index IX_Core_User_DefaultShippingAddressId
    on Core_User (DefaultShippingAddressId);

create table Core_UserAddress
(
    Id          int auto_increment constraint `PRIMARY`
			primary key,
    UserId      int         not null,
    AddressId   int         not null,
    AddressType int         not null,
    LastUsedOn  datetime(6) null,
    IsDeleted   tinyint(1)  not null,
    constraint FK_Core_UserAddress_Core_Address_AddressId
        foreign key (AddressId) references Core_Address (Id),
    constraint FK_Core_UserAddress_Core_User_UserId
        foreign key (UserId) references Core_User (Id)
)
    engine = InnoDB;

alter table Core_User
    add constraint FK_Core_User_Core_UserAddress_DefaultBillingAddressId
        foreign key (DefaultBillingAddressId) references Core_UserAddress (Id);

alter table Core_User
    add constraint FK_Core_User_Core_UserAddress_DefaultShippingAddressId
        foreign key (DefaultShippingAddressId) references Core_UserAddress (Id);

create index IX_Core_UserAddress_AddressId
    on Core_UserAddress (AddressId);

create index IX_Core_UserAddress_UserId
    on Core_UserAddress (UserId);

create table Core_UserClaim
(
    Id         int auto_increment constraint `PRIMARY`
			primary key,
    UserId     int                      not null,
    ClaimType  longtext charset utf8mb4 null,
    ClaimValue longtext charset utf8mb4 null,
    constraint FK_Core_UserClaim_Core_User_UserId
        foreign key (UserId) references Core_User (Id)
            on delete cascade
)
    engine = InnoDB;

create index IX_Core_UserClaim_UserId
    on Core_UserClaim (UserId);

create table Core_UserLogin
(
    LoginProvider       varchar(255) charset utf8mb4 not null,
    ProviderKey         varchar(255) charset utf8mb4 not null,
    ProviderDisplayName longtext charset utf8mb4     null,
    UserId              int                          not null,
    Id                  int                          not null,
    UnionId             varchar(450) charset utf8mb4 null,
    IsDeleted           tinyint(1)                   not null,
    CreatedOn           datetime(6)                  not null,
    UpdatedOn           datetime(6)                  not null,
    constraint `PRIMARY`
        primary key (LoginProvider, ProviderKey),
    constraint FK_Core_UserLogin_Core_User_UserId
        foreign key (UserId) references Core_User (Id)
            on delete cascade
)
    engine = InnoDB;

create index IX_Core_UserLogin_UserId
    on Core_UserLogin (UserId);

create table Core_UserRole
(
    UserId int not null,
    RoleId int not null,
    constraint `PRIMARY`
        primary key (UserId, RoleId),
    constraint FK_Core_UserRole_Core_Role_RoleId
        foreign key (RoleId) references Core_Role (Id),
    constraint FK_Core_UserRole_Core_User_UserId
        foreign key (UserId) references Core_User (Id)
)
    engine = InnoDB;

create index IX_Core_UserRole_RoleId
    on Core_UserRole (RoleId);

create table Core_UserToken
(
    UserId        int                          not null,
    LoginProvider varchar(255) charset utf8mb4 not null,
    Name          varchar(255) charset utf8mb4 not null,
    Value         longtext charset utf8mb4     null,
    constraint `PRIMARY`
        primary key (UserId, LoginProvider, Name),
    constraint FK_Core_UserToken_Core_User_UserId
        foreign key (UserId) references Core_User (Id)
            on delete cascade
)
    engine = InnoDB;

create table Core_Widget
(
    Id                int auto_increment constraint `PRIMARY`
			primary key,
    Name              varchar(450) charset utf8mb4 not null,
    ViewComponentName longtext charset utf8mb4     null,
    CreateUrl         longtext charset utf8mb4     null,
    EditUrl           longtext charset utf8mb4     null,
    IsPublished       tinyint(1)                   not null,
    CreatedOn         datetime(6)                  not null
)
    engine = InnoDB;

create table Core_WidgetZone
(
    Id          int auto_increment constraint `PRIMARY`
			primary key,
    Name        varchar(450) charset utf8mb4 not null,
    Description longtext charset utf8mb4     null,
    CreatedOn   datetime(6)                  not null
)
    engine = InnoDB;

create table Core_WidgetInstance
(
    Id           int auto_increment constraint `PRIMARY`
			primary key,
    Name         longtext charset utf8mb4 null,
    PublishStart datetime(6)              null,
    PublishEnd   datetime(6)              null,
    WidgetId     int                      not null,
    WidgetZoneId int                      not null,
    DisplayOrder int                      not null,
    Data         longtext charset utf8mb4 null,
    HtmlData     longtext charset utf8mb4 null,
    IsDeleted    tinyint(1)               not null,
    CreatedOn    datetime(6)              not null,
    UpdatedOn    datetime(6)              not null,
    constraint FK_Core_WidgetInstance_Core_WidgetZone_WidgetZoneId
        foreign key (WidgetZoneId) references Core_WidgetZone (Id),
    constraint FK_Core_WidgetInstance_Core_Widget_WidgetId
        foreign key (WidgetId) references Core_Widget (Id)
)
    engine = InnoDB;

create index IX_Core_WidgetInstance_WidgetId
    on Core_WidgetInstance (WidgetId);

create index IX_Core_WidgetInstance_WidgetZoneId
    on Core_WidgetInstance (WidgetZoneId);

create table Feedbacks_Feedback
(
    Id        int auto_increment constraint `PRIMARY`
			primary key,
    UserId    int                          null,
    Contact   longtext charset utf8mb4     null,
    Title     varchar(450) charset utf8mb4 null,
    Content   varchar(450) charset utf8mb4 null,
    Type      int                          not null,
    IsDeleted tinyint(1)                   not null,
    CreatedOn datetime(6)                  not null,
    UpdatedOn datetime(6)                  not null,
    constraint FK_Feedbacks_Feedback_Core_User_UserId
        foreign key (UserId) references Core_User (Id)
)
    engine = InnoDB;

create index IX_Feedbacks_Feedback_UserId
    on Feedbacks_Feedback (UserId);

create table Inventory_Warehouse
(
    Id          int auto_increment constraint `PRIMARY`
			primary key,
    Name        varchar(450) charset utf8mb4 not null,
    AddressId   int                          not null,
    AdminRemark longtext charset utf8mb4     null,
    IsDeleted   tinyint(1)                   not null,
    CreatedOn   datetime(6)                  not null,
    UpdatedOn   datetime(6)                  not null,
    constraint FK_Inventory_Warehouse_Core_Address_AddressId
        foreign key (AddressId) references Core_Address (Id)
)
    engine = InnoDB;

create index IX_Inventory_Warehouse_AddressId
    on Inventory_Warehouse (AddressId);

create table Orders_Order
(
    Id                   int auto_increment constraint `PRIMARY`
			primary key,
    No                   bigint                       not null,
    CustomerId           int                          not null,
    ShippingAddressId    int                          null,
    BillingAddressId     int                          null,
    OrderStatus          int                          not null,
    PaymentType          int                          not null,
    ShippingStatus       int                          null,
    ShippedOn            datetime(6)                  null,
    DeliveredOn          datetime(6)                  null,
    DeliveredEndOn       datetime(6)                  null,
    RefundStatus         int                          null,
    RefundReason         longtext charset utf8mb4     null,
    RefundOn             datetime(6)                  null,
    RefundAmount         decimal(65, 30)              not null,
    ShippingMethod       int                          not null,
    ShippingFeeAmount    decimal(65, 30)              not null,
    PaymentMethod        int                          null,
    PaymentFeeAmount     decimal(65, 30)              not null,
    PaymentOn            datetime(6)                  null,
    PaymentEndOn         datetime(6)                  null,
    CouponCode           longtext charset utf8mb4     null,
    CouponRuleName       longtext charset utf8mb4     null,
    SubTotal             decimal(65, 30)              not null,
    SubTotalWithDiscount decimal(65, 30)              not null,
    OrderTotal           decimal(65, 30)              not null,
    DiscountAmount       decimal(65, 30)              not null,
    OrderNote            varchar(450) charset utf8mb4 null,
    AdminNote            varchar(450) charset utf8mb4 null,
    CancelReason         varchar(450) charset utf8mb4 null,
    OnHoldReason         varchar(450) charset utf8mb4 null,
    CancelOn             datetime(6)                  null,
    CreatedById          int                          not null,
    UpdatedById          int                          not null,
    IsDeleted            tinyint(1)                   not null,
    CreatedOn            datetime(6)                  not null,
    UpdatedOn            datetime(6)                  not null,
    constraint IX_Orders_Order_No
        unique (No),
    constraint FK_Orders_Order_Core_User_CreatedById
        foreign key (CreatedById) references Core_User (Id),
    constraint FK_Orders_Order_Core_User_CustomerId
        foreign key (CustomerId) references Core_User (Id),
    constraint FK_Orders_Order_Core_User_UpdatedById
        foreign key (UpdatedById) references Core_User (Id)
)
    engine = InnoDB;

create index IX_Orders_Order_BillingAddressId
    on Orders_Order (BillingAddressId);

create index IX_Orders_Order_CreatedById
    on Orders_Order (CreatedById);

create index IX_Orders_Order_CustomerId
    on Orders_Order (CustomerId);

create index IX_Orders_Order_ShippingAddressId
    on Orders_Order (ShippingAddressId);

create index IX_Orders_Order_UpdatedById
    on Orders_Order (UpdatedById);

create table Orders_OrderAddress
(
    Id                int auto_increment constraint `PRIMARY`
			primary key,
    OrderId           int                      not null,
    ContactName       longtext charset utf8mb4 null,
    Phone             longtext charset utf8mb4 null,
    AddressLine1      longtext charset utf8mb4 null,
    AddressLine2      longtext charset utf8mb4 null,
    City              longtext charset utf8mb4 null,
    ZipCode           longtext charset utf8mb4 null,
    Email             longtext charset utf8mb4 null,
    Company           longtext charset utf8mb4 null,
    AddressType       int                      not null,
    StateOrProvinceId int                      not null,
    CountryId         int                      not null,
    IsDeleted         tinyint(1)               not null,
    CreatedOn         datetime(6)              not null,
    UpdatedOn         datetime(6)              not null,
    constraint FK_Orders_OrderAddress_Core_Country_CountryId
        foreign key (CountryId) references Core_Country (Id),
    constraint FK_Orders_OrderAddress_Core_StateOrProvince_StateOrProvinceId
        foreign key (StateOrProvinceId) references Core_StateOrProvince (Id),
    constraint FK_Orders_OrderAddress_Orders_Order_OrderId
        foreign key (OrderId) references Orders_Order (Id)
            on delete cascade
)
    engine = InnoDB;

alter table Orders_Order
    add constraint FK_Orders_Order_Orders_OrderAddress_BillingAddressId
        foreign key (BillingAddressId) references Orders_OrderAddress (Id);

alter table Orders_Order
    add constraint FK_Orders_Order_Orders_OrderAddress_ShippingAddressId
        foreign key (ShippingAddressId) references Orders_OrderAddress (Id);

create index IX_Orders_OrderAddress_CountryId
    on Orders_OrderAddress (CountryId);

create index IX_Orders_OrderAddress_OrderId
    on Orders_OrderAddress (OrderId);

create index IX_Orders_OrderAddress_StateOrProvinceId
    on Orders_OrderAddress (StateOrProvinceId);

create table Orders_OrderHistory
(
    Id            int auto_increment constraint `PRIMARY`
			primary key,
    OrderId       int                      not null,
    OldStatus     int                      null,
    NewStatus     int                      not null,
    OrderSnapshot longtext charset utf8mb4 null,
    Note          longtext charset utf8mb4 null,
    CreatedById   int                      not null,
    UpdatedById   int                      not null,
    IsDeleted     tinyint(1)               not null,
    CreatedOn     datetime(6)              not null,
    UpdatedOn     datetime(6)              not null,
    constraint FK_Orders_OrderHistory_Core_User_CreatedById
        foreign key (CreatedById) references Core_User (Id),
    constraint FK_Orders_OrderHistory_Core_User_UpdatedById
        foreign key (UpdatedById) references Core_User (Id),
    constraint FK_Orders_OrderHistory_Orders_Order_OrderId
        foreign key (OrderId) references Orders_Order (Id)
)
    engine = InnoDB;

create index IX_Orders_OrderHistory_CreatedById
    on Orders_OrderHistory (CreatedById);

create index IX_Orders_OrderHistory_OrderId
    on Orders_OrderHistory (OrderId);

create index IX_Orders_OrderHistory_UpdatedById
    on Orders_OrderHistory (UpdatedById);

create table Reviews_Review
(
    Id           int auto_increment constraint `PRIMARY`
			primary key,
    UserId       int                          not null,
    Title        varchar(450) charset utf8mb4 null,
    Comment      varchar(450) charset utf8mb4 null,
    Rating       int                          not null,
    ReviewerName varchar(450) charset utf8mb4 null,
    Status       int                          not null,
    EntityTypeId int                          not null,
    EntityId     int                          not null,
    SourceId     int                          null,
    SourceType   int                          null,
    IsAnonymous  tinyint(1)                   not null,
    SupportCount int                          not null,
    IsSystem     tinyint(1)                   not null,
    IsDeleted    tinyint(1)                   not null,
    CreatedOn    datetime(6)                  not null,
    UpdatedOn    datetime(6)                  not null,
    constraint FK_Reviews_Review_Core_User_UserId
        foreign key (UserId) references Core_User (Id)
)
    engine = InnoDB;

create table Reviews_Reply
(
    Id           int auto_increment constraint `PRIMARY`
			primary key,
    ParentId     int                          null,
    ReviewId     int                          not null,
    UserId       int                          not null,
    Comment      varchar(450) charset utf8mb4 null,
    ReplierName  varchar(450) charset utf8mb4 null,
    ToUserId     int                          null,
    ToUserName   varchar(450) charset utf8mb4 null,
    Status       int                          not null,
    IsAnonymous  tinyint(1)                   not null,
    SupportCount int                          not null,
    IsDeleted    tinyint(1)                   not null,
    CreatedOn    datetime(6)                  not null,
    UpdatedOn    datetime(6)                  not null,
    constraint FK_Reviews_Reply_Core_User_ToUserId
        foreign key (ToUserId) references Core_User (Id),
    constraint FK_Reviews_Reply_Core_User_UserId
        foreign key (UserId) references Core_User (Id),
    constraint FK_Reviews_Reply_Reviews_Reply_ParentId
        foreign key (ParentId) references Reviews_Reply (Id),
    constraint FK_Reviews_Reply_Reviews_Review_ReviewId
        foreign key (ReviewId) references Reviews_Review (Id)
)
    engine = InnoDB;

create index IX_Reviews_Reply_ParentId
    on Reviews_Reply (ParentId);

create index IX_Reviews_Reply_ReviewId
    on Reviews_Reply (ReviewId);

create index IX_Reviews_Reply_ToUserId
    on Reviews_Reply (ToUserId);

create index IX_Reviews_Reply_UserId
    on Reviews_Reply (UserId);

create index IX_Reviews_Review_UserId
    on Reviews_Review (UserId);

create table Reviews_ReviewMedia
(
    Id           int auto_increment constraint `PRIMARY`
			primary key,
    ReviewId     int         not null,
    MediaId      int         not null,
    DisplayOrder int         not null,
    IsDeleted    tinyint(1)  not null,
    CreatedOn    datetime(6) not null,
    UpdatedOn    datetime(6) not null,
    constraint FK_Reviews_ReviewMedia_Core_Media_MediaId
        foreign key (MediaId) references Core_Media (Id),
    constraint FK_Reviews_ReviewMedia_Reviews_Review_ReviewId
        foreign key (ReviewId) references Reviews_Review (Id)
)
    engine = InnoDB;

create index IX_Reviews_ReviewMedia_MediaId
    on Reviews_ReviewMedia (MediaId);

create index IX_Reviews_ReviewMedia_ReviewId
    on Reviews_ReviewMedia (ReviewId);

create table Reviews_Support
(
    Id           int auto_increment constraint `PRIMARY`
			primary key,
    UserId       int         not null,
    EntityTypeId int         not null,
    EntityId     int         not null,
    IsDeleted    tinyint(1)  not null,
    CreatedOn    datetime(6) not null,
    UpdatedOn    datetime(6) not null,
    ReviewId     int         null,
    constraint FK_Reviews_Support_Core_User_UserId
        foreign key (UserId) references Core_User (Id),
    constraint FK_Reviews_Support_Reviews_Review_ReviewId
        foreign key (ReviewId) references Reviews_Review (Id)
)
    engine = InnoDB;

create index IX_Reviews_Support_ReviewId
    on Reviews_Support (ReviewId);

create index IX_Reviews_Support_UserId
    on Reviews_Support (UserId);

create table Shipments_Shipment
(
    Id             int auto_increment constraint `PRIMARY`
			primary key,
    OrderId        int                          not null,
    TotalWeight    decimal(65, 30)              not null,
    TrackingNumber varchar(450) charset utf8mb4 null,
    ShippedOn      datetime(6)                  null,
    DeliveredOn    datetime(6)                  null,
    AdminComment   longtext charset utf8mb4     null,
    CreatedById    int                          not null,
    UpdatedById    int                          not null,
    IsDeleted      tinyint(1)                   not null,
    CreatedOn      datetime(6)                  not null,
    UpdatedOn      datetime(6)                  not null,
    constraint FK_Shipments_Shipment_Core_User_CreatedById
        foreign key (CreatedById) references Core_User (Id),
    constraint FK_Shipments_Shipment_Core_User_UpdatedById
        foreign key (UpdatedById) references Core_User (Id),
    constraint FK_Shipments_Shipment_Orders_Order_OrderId
        foreign key (OrderId) references Orders_Order (Id)
)
    engine = InnoDB;

create index IX_Shipments_Shipment_CreatedById
    on Shipments_Shipment (CreatedById);

create index IX_Shipments_Shipment_IsDeleted
    on Shipments_Shipment (IsDeleted);

create index IX_Shipments_Shipment_OrderId
    on Shipments_Shipment (OrderId);

create index IX_Shipments_Shipment_TrackingNumber
    on Shipments_Shipment (TrackingNumber);

create index IX_Shipments_Shipment_UpdatedById
    on Shipments_Shipment (UpdatedById);

create table Shipping_FreightTemplate
(
    Id        int auto_increment constraint `PRIMARY`
			primary key,
    Name      varchar(450) charset utf8mb4 null,
    Note      longtext charset utf8mb4     null,
    IsDeleted tinyint(1)                   not null,
    CreatedOn datetime(6)                  not null,
    UpdatedOn datetime(6)                  not null
)
    engine = InnoDB;

create table Catalog_Product
(
    Id                       int auto_increment constraint `PRIMARY`
			primary key,
    ParentGroupedProductId   int                          null,
    Name                     varchar(450) charset utf8mb4 not null,
    Slug                     varchar(450) charset utf8mb4 not null,
    MetaTitle                longtext charset utf8mb4     null,
    MetaKeywords             longtext charset utf8mb4     null,
    MetaDescription          longtext charset utf8mb4     null,
    ShortDescription         longtext charset utf8mb4     null,
    Description              longtext charset utf8mb4     null,
    Specification            longtext charset utf8mb4     null,
    Price                    decimal(65, 30)              not null,
    OldPrice                 decimal(65, 30)              null,
    SpecialPrice             decimal(65, 30)              null,
    SpecialPriceStart        datetime(6)                  null,
    SpecialPriceEnd          datetime(6)                  null,
    HasOptions               tinyint(1)                   not null,
    IsVisibleIndividually    tinyint(1)                   not null,
    IsFeatured               tinyint(1)                   not null,
    IsCallForPricing         tinyint(1)                   not null,
    IsAllowToOrder           tinyint(1)                   not null,
    StockTrackingIsEnabled   tinyint(1)                   not null,
    Sku                      varchar(450) charset utf8mb4 null,
    Gtin                     varchar(450) charset utf8mb4 null,
    NormalizedName           varchar(450) charset utf8mb4 null,
    ThumbnailImageId         int                          null,
    ReviewsCount             int                          not null,
    RatingAverage            double                       null,
    BrandId                  int                          null,
    Barcode                  longtext charset utf8mb4     null,
    ValidThru                int                          null,
    OrderMinimumQuantity     int                          not null,
    OrderMaximumQuantity     int                          not null,
    DisplayStockAvailability tinyint(1)                   not null,
    DisplayStockQuantity     tinyint(1)                   not null,
    StockReduceStrategy      int                          not null,
    NotReturnable            tinyint(1)                   not null,
    PublishType              int                          not null,
    DisplayOrder             int                          not null,
    IsPublished              tinyint(1)                   not null,
    PublishedOn              datetime(6)                  null,
    UnpublishedOn            datetime(6)                  null,
    UnpublishedReason        longtext charset utf8mb4     null,
    IsShipEnabled            tinyint(1)                   not null,
    Weight                   decimal(65, 30)              not null,
    Length                   decimal(65, 30)              not null,
    Width                    decimal(65, 30)              not null,
    Height                   decimal(65, 30)              not null,
    IsFreeShipping           tinyint(1)                   not null,
    AdditionalShippingCharge decimal(65, 30)              not null,
    FreightTemplateId        int                          null,
    UnitId                   int                          null,
    AdminRemark              longtext charset utf8mb4     null,
    DeliveryTime             int                          null,
    IsDeleted                tinyint(1)                   not null,
    CreatedById              int                          not null,
    CreatedOn                datetime(6)                  not null,
    UpdatedById              int                          not null,
    UpdatedOn                datetime(6)                  not null,
    constraint FK_Catalog_Product_Catalog_Brand_BrandId
        foreign key (BrandId) references Catalog_Brand (Id),
    constraint FK_Catalog_Product_Catalog_Product_ParentGroupedProductId
        foreign key (ParentGroupedProductId) references Catalog_Product (Id),
    constraint FK_Catalog_Product_Catalog_Unit_UnitId
        foreign key (UnitId) references Catalog_Unit (Id),
    constraint FK_Catalog_Product_Core_Media_ThumbnailImageId
        foreign key (ThumbnailImageId) references Core_Media (Id),
    constraint FK_Catalog_Product_Core_User_CreatedById
        foreign key (CreatedById) references Core_User (Id),
    constraint FK_Catalog_Product_Core_User_UpdatedById
        foreign key (UpdatedById) references Core_User (Id),
    constraint FK_Catalog_Product_Shipping_FreightTemplate_FreightTemplateId
        foreign key (FreightTemplateId) references Shipping_FreightTemplate (Id)
)
    engine = InnoDB;

create index IX_Catalog_Product_BrandId
    on Catalog_Product (BrandId);

create index IX_Catalog_Product_CreatedById
    on Catalog_Product (CreatedById);

create index IX_Catalog_Product_FreightTemplateId
    on Catalog_Product (FreightTemplateId);

create index IX_Catalog_Product_IsDeleted
    on Catalog_Product (IsDeleted);

create index IX_Catalog_Product_IsPublished
    on Catalog_Product (IsPublished);

create index IX_Catalog_Product_Name
    on Catalog_Product (Name);

create index IX_Catalog_Product_ParentGroupedProductId
    on Catalog_Product (ParentGroupedProductId);

create index IX_Catalog_Product_Sku
    on Catalog_Product (Sku);

create index IX_Catalog_Product_Slug
    on Catalog_Product (Slug);

create index IX_Catalog_Product_ThumbnailImageId
    on Catalog_Product (ThumbnailImageId);

create index IX_Catalog_Product_UnitId
    on Catalog_Product (UnitId);

create index IX_Catalog_Product_UpdatedById
    on Catalog_Product (UpdatedById);

create table Catalog_ProductAttributeValue
(
    Id          int auto_increment constraint `PRIMARY`
			primary key,
    AttributeId int                      not null,
    ProductId   int                      not null,
    Value       longtext charset utf8mb4 null,
    Description longtext charset utf8mb4 null,
    IsDeleted   tinyint(1)               not null,
    CreatedOn   datetime(6)              not null,
    UpdatedOn   datetime(6)              not null,
    constraint `FK_Catalog_ProductAttributeValue_Catalog_ProductAttribute_Attri~`
        foreign key (AttributeId) references Catalog_ProductAttribute (Id),
    constraint FK_Catalog_ProductAttributeValue_Catalog_Product_ProductId
        foreign key (ProductId) references Catalog_Product (Id)
)
    engine = InnoDB;

create index IX_Catalog_ProductAttributeValue_AttributeId
    on Catalog_ProductAttributeValue (AttributeId);

create index IX_Catalog_ProductAttributeValue_ProductId
    on Catalog_ProductAttributeValue (ProductId);

create table Catalog_ProductCategory
(
    Id                int auto_increment constraint `PRIMARY`
			primary key,
    CategoryId        int         not null,
    ProductId         int         not null,
    IsFeaturedProduct tinyint(1)  not null,
    DisplayOrder      int         not null,
    IsDeleted         tinyint(1)  not null,
    CreatedOn         datetime(6) not null,
    UpdatedOn         datetime(6) not null,
    constraint FK_Catalog_ProductCategory_Catalog_Category_CategoryId
        foreign key (CategoryId) references Catalog_Category (Id),
    constraint FK_Catalog_ProductCategory_Catalog_Product_ProductId
        foreign key (ProductId) references Catalog_Product (Id)
)
    engine = InnoDB;

create index IX_Catalog_ProductCategory_CategoryId
    on Catalog_ProductCategory (CategoryId);

create index IX_Catalog_ProductCategory_ProductId
    on Catalog_ProductCategory (ProductId);

create table Catalog_ProductLink
(
    Id              int auto_increment constraint `PRIMARY`
			primary key,
    ProductId       int         not null,
    LinkedProductId int         not null,
    LinkType        int         not null,
    IsDeleted       tinyint(1)  not null,
    CreatedOn       datetime(6) not null,
    UpdatedOn       datetime(6) not null,
    constraint FK_Catalog_ProductLink_Catalog_Product_LinkedProductId
        foreign key (LinkedProductId) references Catalog_Product (Id),
    constraint FK_Catalog_ProductLink_Catalog_Product_ProductId
        foreign key (ProductId) references Catalog_Product (Id)
)
    engine = InnoDB;

create index IX_Catalog_ProductLink_LinkedProductId
    on Catalog_ProductLink (LinkedProductId);

create index IX_Catalog_ProductLink_ProductId
    on Catalog_ProductLink (ProductId);

create table Catalog_ProductMedia
(
    Id           int auto_increment constraint `PRIMARY`
			primary key,
    ProductId    int         not null,
    MediaId      int         not null,
    DisplayOrder int         not null,
    IsDeleted    tinyint(1)  not null,
    CreatedOn    datetime(6) not null,
    UpdatedOn    datetime(6) not null,
    constraint FK_Catalog_ProductMedia_Catalog_Product_ProductId
        foreign key (ProductId) references Catalog_Product (Id),
    constraint FK_Catalog_ProductMedia_Core_Media_MediaId
        foreign key (MediaId) references Core_Media (Id)
)
    engine = InnoDB;

create index IX_Catalog_ProductMedia_MediaId
    on Catalog_ProductMedia (MediaId);

create index IX_Catalog_ProductMedia_ProductId
    on Catalog_ProductMedia (ProductId);

create table Catalog_ProductOptionCombination
(
    Id           int auto_increment constraint `PRIMARY`
			primary key,
    ProductId    int                          not null,
    OptionId     int                          not null,
    DisplayOrder int                          not null,
    Value        varchar(450) charset utf8mb4 null,
    IsDeleted    tinyint(1)                   not null,
    CreatedOn    datetime(6)                  not null,
    UpdatedOn    datetime(6)                  not null,
    constraint `FK_Catalog_ProductOptionCombination_Catalog_ProductOption_Optio~`
        foreign key (OptionId) references Catalog_ProductOption (Id),
    constraint FK_Catalog_ProductOptionCombination_Catalog_Product_ProductId
        foreign key (ProductId) references Catalog_Product (Id)
)
    engine = InnoDB;

create index IX_Catalog_ProductOptionCombination_OptionId
    on Catalog_ProductOptionCombination (OptionId);

create index IX_Catalog_ProductOptionCombination_ProductId
    on Catalog_ProductOptionCombination (ProductId);

create table Catalog_ProductOptionValue
(
    Id           int auto_increment constraint `PRIMARY`
			primary key,
    OptionId     int                          not null,
    ProductId    int                          not null,
    Value        varchar(450) charset utf8mb4 null,
    Display      varchar(450) charset utf8mb4 null,
    DisplayOrder int                          not null,
    MediaId      int                          null,
    IsDefault    tinyint(1)                   not null,
    IsDeleted    tinyint(1)                   not null,
    CreatedOn    datetime(6)                  not null,
    UpdatedOn    datetime(6)                  not null,
    constraint FK_Catalog_ProductOptionValue_Catalog_ProductOption_OptionId
        foreign key (OptionId) references Catalog_ProductOption (Id),
    constraint FK_Catalog_ProductOptionValue_Catalog_Product_ProductId
        foreign key (ProductId) references Catalog_Product (Id),
    constraint FK_Catalog_ProductOptionValue_Core_Media_MediaId
        foreign key (MediaId) references Core_Media (Id)
)
    engine = InnoDB;

create index IX_Catalog_ProductOptionValue_MediaId
    on Catalog_ProductOptionValue (MediaId);

create index IX_Catalog_ProductOptionValue_OptionId
    on Catalog_ProductOptionValue (OptionId);

create index IX_Catalog_ProductOptionValue_ProductId
    on Catalog_ProductOptionValue (ProductId);

create table Catalog_ProductPriceHistory
(
    Id                int auto_increment constraint `PRIMARY`
			primary key,
    ProductId         int             null,
    Price             decimal(65, 30) null,
    OldPrice          decimal(65, 30) null,
    SpecialPrice      decimal(65, 30) null,
    SpecialPriceStart datetime(6)     null,
    SpecialPriceEnd   datetime(6)     null,
    IsDeleted         tinyint(1)      not null,
    CreatedById       int             not null,
    UpdatedById       int             not null,
    CreatedOn         datetime(6)     not null,
    UpdatedOn         datetime(6)     not null,
    constraint FK_Catalog_ProductPriceHistory_Catalog_Product_ProductId
        foreign key (ProductId) references Catalog_Product (Id),
    constraint FK_Catalog_ProductPriceHistory_Core_User_CreatedById
        foreign key (CreatedById) references Core_User (Id),
    constraint FK_Catalog_ProductPriceHistory_Core_User_UpdatedById
        foreign key (UpdatedById) references Core_User (Id)
)
    engine = InnoDB;

create index IX_Catalog_ProductPriceHistory_CreatedById
    on Catalog_ProductPriceHistory (CreatedById);

create index IX_Catalog_ProductPriceHistory_ProductId
    on Catalog_ProductPriceHistory (ProductId);

create index IX_Catalog_ProductPriceHistory_UpdatedById
    on Catalog_ProductPriceHistory (UpdatedById);

create table Catalog_ProductRecentlyViewed
(
    Id             int auto_increment constraint `PRIMARY`
			primary key,
    ProductId      int         not null,
    CustomerId     int         not null,
    ViewedCount    int         not null,
    IsDeleted      tinyint(1)  not null,
    CreatedOn      datetime(6) not null,
    LatestViewedOn datetime(6) not null,
    constraint FK_Catalog_ProductRecentlyViewed_Catalog_Product_ProductId
        foreign key (ProductId) references Catalog_Product (Id),
    constraint FK_Catalog_ProductRecentlyViewed_Core_User_CustomerId
        foreign key (CustomerId) references Core_User (Id)
)
    engine = InnoDB;

create index IX_Catalog_ProductRecentlyViewed_CustomerId
    on Catalog_ProductRecentlyViewed (CustomerId);

create index IX_Catalog_ProductRecentlyViewed_ProductId
    on Catalog_ProductRecentlyViewed (ProductId);

create table Catalog_ProductWishlist
(
    Id          int auto_increment constraint `PRIMARY`
			primary key,
    ProductId   int                          not null,
    CustomerId  int                          not null,
    Quantity    int                          not null,
    Description varchar(450) charset utf8mb4 null,
    IsDeleted   tinyint(1)                   not null,
    CreatedOn   datetime(6)                  not null,
    UpdatedOn   datetime(6)                  not null,
    constraint FK_Catalog_ProductWishlist_Catalog_Product_ProductId
        foreign key (ProductId) references Catalog_Product (Id),
    constraint FK_Catalog_ProductWishlist_Core_User_CustomerId
        foreign key (CustomerId) references Core_User (Id)
)
    engine = InnoDB;

create index IX_Catalog_ProductWishlist_CustomerId
    on Catalog_ProductWishlist (CustomerId);

create index IX_Catalog_ProductWishlist_ProductId
    on Catalog_ProductWishlist (ProductId);

create table Inventory_Stock
(
    Id                  int auto_increment constraint `PRIMARY`
			primary key,
    StockQuantity       int                          not null,
    LockedStockQuantity int                          not null,
    ProductId           int                          not null,
    WarehouseId         int                          not null,
    DisplayOrder        int                          not null,
    IsEnabled           tinyint(1)                   not null,
    Note                varchar(450) charset utf8mb4 null,
    IsDeleted           tinyint(1)                   not null,
    CreatedOn           datetime(6)                  not null,
    UpdatedOn           datetime(6)                  not null,
    constraint FK_Inventory_Stock_Catalog_Product_ProductId
        foreign key (ProductId) references Catalog_Product (Id),
    constraint FK_Inventory_Stock_Inventory_Warehouse_WarehouseId
        foreign key (WarehouseId) references Inventory_Warehouse (Id)
)
    engine = InnoDB;

create index IX_Inventory_Stock_ProductId
    on Inventory_Stock (ProductId);

create index IX_Inventory_Stock_WarehouseId
    on Inventory_Stock (WarehouseId);

create table Inventory_StockHistory
(
    Id               int auto_increment constraint `PRIMARY`
			primary key,
    ProductId        int                      not null,
    WarehouseId      int                      not null,
    AdjustedQuantity int                      not null,
    StockQuantity    int                      not null,
    Note             longtext charset utf8mb4 null,
    IsDeleted        tinyint(1)               not null,
    CreatedOn        datetime(6)              not null,
    CreatedById      int                      not null,
    UpdatedOn        datetime(6)              not null,
    UpdatedById      int                      not null,
    constraint FK_Inventory_StockHistory_Catalog_Product_ProductId
        foreign key (ProductId) references Catalog_Product (Id),
    constraint FK_Inventory_StockHistory_Core_User_CreatedById
        foreign key (CreatedById) references Core_User (Id),
    constraint FK_Inventory_StockHistory_Core_User_UpdatedById
        foreign key (UpdatedById) references Core_User (Id),
    constraint FK_Inventory_StockHistory_Inventory_Warehouse_WarehouseId
        foreign key (WarehouseId) references Inventory_Warehouse (Id)
)
    engine = InnoDB;

create index IX_Inventory_StockHistory_CreatedById
    on Inventory_StockHistory (CreatedById);

create index IX_Inventory_StockHistory_ProductId
    on Inventory_StockHistory (ProductId);

create index IX_Inventory_StockHistory_UpdatedById
    on Inventory_StockHistory (UpdatedById);

create index IX_Inventory_StockHistory_WarehouseId
    on Inventory_StockHistory (WarehouseId);

create table Orders_OrderItem
(
    Id              int auto_increment constraint `PRIMARY`
			primary key,
    OrderId         int                      not null,
    ProductId       int                      not null,
    ProductPrice    decimal(65, 30)          not null,
    ProductName     longtext charset utf8mb4 null,
    ProductMediaUrl longtext charset utf8mb4 null,
    Quantity        int                      not null,
    ShippedQuantity int                      not null,
    DiscountAmount  decimal(65, 30)          not null,
    ItemAmount      decimal(65, 30)          not null,
    ItemWeight      decimal(65, 30)          not null,
    Note            longtext charset utf8mb4 null,
    CreatedById     int                      not null,
    UpdatedById     int                      not null,
    IsDeleted       tinyint(1)               not null,
    CreatedOn       datetime(6)              not null,
    UpdatedOn       datetime(6)              not null,
    constraint FK_Orders_OrderItem_Catalog_Product_ProductId
        foreign key (ProductId) references Catalog_Product (Id),
    constraint FK_Orders_OrderItem_Core_User_CreatedById
        foreign key (CreatedById) references Core_User (Id),
    constraint FK_Orders_OrderItem_Core_User_UpdatedById
        foreign key (UpdatedById) references Core_User (Id),
    constraint FK_Orders_OrderItem_Orders_Order_OrderId
        foreign key (OrderId) references Orders_Order (Id)
)
    engine = InnoDB;

create index IX_Orders_OrderItem_CreatedById
    on Orders_OrderItem (CreatedById);

create index IX_Orders_OrderItem_OrderId
    on Orders_OrderItem (OrderId);

create index IX_Orders_OrderItem_ProductId
    on Orders_OrderItem (ProductId);

create index IX_Orders_OrderItem_UpdatedById
    on Orders_OrderItem (UpdatedById);

create table Shipments_ShipmentItem
(
    Id          int auto_increment constraint `PRIMARY`
			primary key,
    ShipmentId  int         not null,
    OrderItemId int         not null,
    ProductId   int         not null,
    Quantity    int         not null,
    CreatedById int         not null,
    UpdatedById int         not null,
    IsDeleted   tinyint(1)  not null,
    CreatedOn   datetime(6) not null,
    UpdatedOn   datetime(6) not null,
    constraint FK_Shipments_ShipmentItem_Catalog_Product_ProductId
        foreign key (ProductId) references Catalog_Product (Id),
    constraint FK_Shipments_ShipmentItem_Core_User_CreatedById
        foreign key (CreatedById) references Core_User (Id),
    constraint FK_Shipments_ShipmentItem_Core_User_UpdatedById
        foreign key (UpdatedById) references Core_User (Id),
    constraint FK_Shipments_ShipmentItem_Orders_OrderItem_OrderItemId
        foreign key (OrderItemId) references Orders_OrderItem (Id),
    constraint FK_Shipments_ShipmentItem_Shipments_Shipment_ShipmentId
        foreign key (ShipmentId) references Shipments_Shipment (Id)
)
    engine = InnoDB;

create index IX_Shipments_ShipmentItem_CreatedById
    on Shipments_ShipmentItem (CreatedById);

create index IX_Shipments_ShipmentItem_OrderItemId
    on Shipments_ShipmentItem (OrderItemId);

create index IX_Shipments_ShipmentItem_ProductId
    on Shipments_ShipmentItem (ProductId);

create index IX_Shipments_ShipmentItem_ShipmentId
    on Shipments_ShipmentItem (ShipmentId);

create index IX_Shipments_ShipmentItem_UpdatedById
    on Shipments_ShipmentItem (UpdatedById);

create table Shipping_PriceAndDestination
(
    Id                int auto_increment constraint `PRIMARY`
			primary key,
    FreightTemplateId int                      not null,
    CountryId         int                      not null,
    StateOrProvinceId int                      null,
    MinOrderSubtotal  decimal(65, 30)          not null,
    ShippingPrice     decimal(65, 30)          not null,
    Note              longtext charset utf8mb4 null,
    IsEnabled         tinyint(1)               not null,
    IsDeleted         tinyint(1)               not null,
    CreatedOn         datetime(6)              not null,
    UpdatedOn         datetime(6)              not null,
    constraint FK_Shipping_PriceAndDestination_Core_Country_CountryId
        foreign key (CountryId) references Core_Country (Id),
    constraint `FK_Shipping_PriceAndDestination_Core_StateOrProvince_StateOrPro~`
        foreign key (StateOrProvinceId) references Core_StateOrProvince (Id),
    constraint `FK_Shipping_PriceAndDestination_Shipping_FreightTemplate_Freigh~`
        foreign key (FreightTemplateId) references Shipping_FreightTemplate (Id)
)
    engine = InnoDB;

create index IX_Shipping_PriceAndDestination_CountryId
    on Shipping_PriceAndDestination (CountryId);

create index IX_Shipping_PriceAndDestination_FreightTemplateId
    on Shipping_PriceAndDestination (FreightTemplateId);

create index IX_Shipping_PriceAndDestination_StateOrProvinceId
    on Shipping_PriceAndDestination (StateOrProvinceId);

create table ShoppingCart_Cart
(
    Id                       int auto_increment constraint `PRIMARY`
			primary key,
    CustomerId               int                      not null,
    IsActive                 tinyint(1)               not null,
    CouponCode               longtext charset utf8mb4 null,
    CouponRuleName           longtext charset utf8mb4 null,
    ShippingMethod           longtext charset utf8mb4 null,
    IsProductPriceIncludeTax tinyint(1)               not null,
    ShippingAmount           decimal(65, 30)          null,
    ShippingData             longtext charset utf8mb4 null,
    OrderNote                longtext charset utf8mb4 null,
    IsDeleted                tinyint(1)               not null,
    CreatedOn                datetime(6)              not null,
    UpdatedOn                datetime(6)              not null,
    CreatedById              int                      not null,
    UpdatedById              int                      not null,
    constraint FK_ShoppingCart_Cart_Core_User_CreatedById
        foreign key (CreatedById) references Core_User (Id),
    constraint FK_ShoppingCart_Cart_Core_User_CustomerId
        foreign key (CustomerId) references Core_User (Id),
    constraint FK_ShoppingCart_Cart_Core_User_UpdatedById
        foreign key (UpdatedById) references Core_User (Id)
)
    engine = InnoDB;

create index IX_ShoppingCart_Cart_CreatedById
    on ShoppingCart_Cart (CreatedById);

create index IX_ShoppingCart_Cart_CustomerId
    on ShoppingCart_Cart (CustomerId);

create index IX_ShoppingCart_Cart_UpdatedById
    on ShoppingCart_Cart (UpdatedById);

create table ShoppingCart_CartItem
(
    Id          int auto_increment constraint `PRIMARY`
			primary key,
    ProductId   int         not null,
    Quantity    int         not null,
    CartId      int         not null,
    IsChecked   tinyint(1)  not null,
    IsDeleted   tinyint(1)  not null,
    CreatedOn   datetime(6) not null,
    UpdatedOn   datetime(6) not null,
    CreatedById int         not null,
    UpdatedById int         not null,
    constraint FK_ShoppingCart_CartItem_Catalog_Product_ProductId
        foreign key (ProductId) references Catalog_Product (Id),
    constraint FK_ShoppingCart_CartItem_Core_User_CreatedById
        foreign key (CreatedById) references Core_User (Id),
    constraint FK_ShoppingCart_CartItem_Core_User_UpdatedById
        foreign key (UpdatedById) references Core_User (Id),
    constraint FK_ShoppingCart_CartItem_ShoppingCart_Cart_CartId
        foreign key (CartId) references ShoppingCart_Cart (Id)
)
    engine = InnoDB;

create index IX_ShoppingCart_CartItem_CartId
    on ShoppingCart_CartItem (CartId);

create index IX_ShoppingCart_CartItem_CreatedById
    on ShoppingCart_CartItem (CreatedById);

create index IX_ShoppingCart_CartItem_ProductId
    on ShoppingCart_CartItem (ProductId);

create index IX_ShoppingCart_CartItem_UpdatedById
    on ShoppingCart_CartItem (UpdatedById);

create table __EFMigrationsHistory
(
    MigrationId    varchar(95) not null constraint `PRIMARY`
        primary key,
    ProductVersion varchar(32) not null
)
    engine = InnoDB;

s