# Office Service

OfficeService là một dự án quản lý tài liệu văn phòng được xây dựng trên nền tảng .NET 8, sử dụng kiến trúc đa tầng và Entity Framework Core với PostgreSQL.

## Kiến trúc dự án

Dự án được chia thành 4 layer chính:

### 1. OfficeService (API Layer)
- Chứa các API endpoint và cấu hình ứng dụng
- Sử dụng JWT Bearer Authentication
- Swagger UI cho API documentation
- Controllers:
  - BaseController: Controller cơ sở với các thao tác chung
  - FileController: Quản lý tài liệu và phiên bản

### 2. OfficeService.Business (Business Layer)
- Xử lý nghiệp vụ (business logic)
- Các service chính:
  - BaseService: Service cơ sở với các thao tác CRUD
  - FileService: Xử lý nghiệp vụ về tài liệu
  - CachingService: Quản lý cache
  - JWTContext: Xử lý JWT token

### 3. OfficeService.DAL (Data Access Layer)
- Quản lý truy cập dữ liệu với Entity Framework Core
- Repository pattern
- Entities:
  - File: Thông tin tài liệu
  - FileVersion: Quản lý phiên bản tài liệu
  - AuditEntity: Entity cơ sở với các trường audit
- Repositories:
  - GenericRepository: Repository cơ sở
  - FileRepository: Xử lý dữ liệu tài liệu
  - FileVersionRepository: Xử lý phiên bản tài liệu

### 4. OfficeService.Common
- Chứa các thành phần dùng chung
- Constants: Hằng số 
- Enums: Các enum dùng chung

## Các tính năng chính

- Quản lý tài liệu văn phòng
- Theo dõi phiên bản tài liệu
- Xác thực và phân quyền với JWT
- Hỗ trợ caching
- Soft delete
- Audit logging

## Cấu hình và Deployment

### Yêu cầu hệ thống
- .NET 8 SDK
- PostgreSQL 
- Visual Studio 2022 hoặc VS Code

### Cấu hình database
Database PostgreSQL được cấu hình trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "postgres": "User ID=admin;Password=admin_password;Server=localhost;Port=5436;Database=dpu_office;Pooling=true;"
  }
}
```

### Cấu hình JWT
```json
{
  "AuthSetting": {
    "SecretKey": "DlhXHPrSJgIzqZzhK0nRrVPuOo4nhzVF",
    "Authority": "https://localhost:7153"
  }
}
```

### Các bước triển khai

1. Clone repository
2. Restore packages:
```sh
dotnet restore
```

3. Cập nhật database:
```sh
dotnet ef database update --project OfficeService.DAL
```

4. Chạy ứng dụng:
```sh
dotnet run --project OfficeService
```

Ứng dụng sẽ chạy tại:
- HTTP: http://localhost:5126
- HTTPS: https://localhost:7162
- Swagger UI: https://localhost:7162/swagger

## Contributing
Vui lòng liên hệ team phát triển để được hướng dẫn về coding convention và quy trình đóng góp mã nguồn.