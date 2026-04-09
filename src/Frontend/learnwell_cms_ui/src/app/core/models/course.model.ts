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