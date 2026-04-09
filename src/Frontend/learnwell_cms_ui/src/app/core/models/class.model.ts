export interface ClassBase {
  classId: string;
  name: string;
  description: string | null;
}

export interface Class extends ClassBase {
  createdAt: string;
  createdBy: string;
  modifiedAt: string | null;
  modifiedBy: string;
}

export interface CreateUpdateClassRequest {
  name: string;
  description?: string;
}

export interface AddRemoveStudentsRequest {
  addStudentIds: string[];
  removeStudentIds: string[];
}