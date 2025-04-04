import { Component, OnInit } from '@angular/core';
import { UserService } from '../../core/services/user.service';
import { UserResponse } from '../../core/models/users/user.response';
import { UserFilterRequest } from '../../core/models/users/user.filter.request';

// PrimeNG
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { PasswordModule } from 'primeng/password';
import { DropdownModule } from 'primeng/dropdown';
import { DividerModule } from 'primeng/divider';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { UserRequest } from '../../core/models/users/user.request';
import { RoleResponse } from '../../core/models/roles/role.response';
import { environment } from '../../../environments/environment';
import { ExcelService } from '../../core/services/excel.service';
import { FileUploadModule } from 'primeng/fileupload';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    TableModule,
    ButtonModule,
    InputTextModule,
    PasswordModule,
    DropdownModule,
    DividerModule,
    FormsModule,
    ProgressSpinnerModule,
    FileUploadModule,
    AvatarModule,
    AvatarGroupModule,
    DialogModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  
  [x: string]: any;
  users: UserResponse[] = [];
  roles: RoleResponse[] = [];
  selectedUserId: string = '';
  isLoading: boolean = false; 
  baseUrl = environment.baseUrl;
  selectedImageFile: File | null = null;
  imagePreviewUrl: string | null = null;

  totalRecords: number = 0;
  pageSize: number = 10;
  visible = false;
  filterRequest: UserFilterRequest = {
    PageNumber: 1,
    PageSize: 10,
    SortColumn: 'Name',
    SortOrder: true
  };

  user: UserRequest = {
    name: '',
    email: '',
    userName: '',
    roleId: '',
    password: '',
    profileImage: null
  };

  constructor(private userService: UserService, private excelService: ExcelService) { }

  async ngOnInit() {
    await this.fetchUsers();
    this.roles = await this.userService.getRoles();
  }

  async fetchUsers() {
    this.isLoading = true;
    try {
      const response = await this.userService.getUsers(this.filterRequest);
      this.users = response.data;
      this.totalRecords = response.totalCount;
    } 
    finally {
      this.isLoading = false;
    }
  }

  onPageChange(event: any) {
    this.filterRequest.PageNumber = event.first / event.rows + 1;
    this.filterRequest.PageSize = event.rows;
    this.fetchUsers();
  }

  onSortChange(event: any) {
    if (event.field) {
      this.filterRequest.SortColumn = event.field.charAt(0).toUpperCase() + event.field.slice(1);
      this.filterRequest.SortOrder = event.order === 1;
    }
    this.fetchUsers();
  }

  filter(field: string, event: any) {
    const value = event.target.value?.trim().toLowerCase() || undefined;
    console.log(`Filtering by ${field}:`, value);

    if (!value) {
      delete this.filterRequest[field as keyof UserFilterRequest];
    } else {
      this.filterRequest[field as keyof UserFilterRequest] = value;
    }

    this.fetchUsers();
  }

  onFilterChange(event: any) {
    console.log("test");
  }

  openDialog(user?: UserResponse) {
    this.user = {
      name: user?.name || '',
      email: user?.email || '',
      userName: user?.userName || '',
      roleId: user?.role?.id || '',
      password: '',
      profileImage : null
    };
    this.selectedUserId = user?.id || '';
    this.imagePreviewUrl = this.baseUrl + "/"+ user?.profileImageUrl || null;
    this.visible = true;
  }

  async deleteUser(id: string) {
    var result = await this.userService.deleteUser(id);
    if (result) {
      this.fetchUsers();
    }
    else {
      alert("Error deleting user");
    }
  }

  async saveUser() {
    const formData = new FormData();
    formData.append('Name', this.user.name);
    formData.append('Email', this.user.email);
    formData.append('UserName', this.user.userName);
    formData.append('RoleId', this.user.roleId);
    formData.append('Password', this.user.password);
  
    if (this.selectedImageFile) {
      formData.append('ProfileImage', this.selectedImageFile);
    }
  
    if (this.selectedUserId) {
      await this.userService.updateUser(this.selectedUserId, formData);
    } else {
      await this.userService.createUser(formData);
    }
  
    this.visible = false;
    this.selectedImageFile = null;
    this.imagePreviewUrl = null;
    await this.fetchUsers();
  }

  handleImageSelection(event: any): void {
    const file = event.files?.[0];
    if (file) {
      this.selectedImageFile = file;

      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imagePreviewUrl = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  async onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (!file) return;

    this.isLoading = true;

    try {
      await this.excelService.uploadExcel(file);
      console.log('Upload successful');

    } catch (error) {
      console.error('Upload failed', error);
    } finally {
      await this.fetchUsers();
      this.isLoading = false;
    }
  }

  async downloadExcel(){
    this.isLoading = true;
    try {
      await this.excelService.downloadAndOpenExcel();
    } 
    finally {
      await this.fetchUsers();
      this.isLoading = false;
    }
  }
}