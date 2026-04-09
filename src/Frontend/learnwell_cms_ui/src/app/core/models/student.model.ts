export interface StudentBase {
  studentId: string;
  firstName: string;
  lastName: string;
  emailAddress: string | null;
  phoneNumber: string | null;
 }

 export interface CreateUpdateStudentRequest {
  firstName: string;
  lastName: string;
  emailAddress: string;
  phoneNumber: string;
  address: string;
}

export interface Student extends StudentBase {
  createdAt: string;
  createdBy: string;
  modifiedAt: string | null;
  modifiedBy: string;
}

export interface CreateStudentResponse {
  id: string;
  createdBy: string;
  createdAt: string;
  password: string;
}
