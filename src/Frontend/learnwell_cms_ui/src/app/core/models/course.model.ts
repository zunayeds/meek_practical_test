export interface CourseBase {
  courseId: string;
  name: string;
  description: string | null;
}

export interface Course extends CourseBase {
  createdAt: string;
  createdBy: string;
  modifiedAt: string | null;
  modifiedBy: string;
}

export interface CreateUpdateCourseRequest {
  name: string;
  description?: string;
}

export interface AddRemoveClassesRequest {
  addClassIds: string[];
  removeClassIds: string[];
}

export interface AddRemoveStudentsRequest {
  addStudentIds: string[];
  removeStudentIds: string[];
}