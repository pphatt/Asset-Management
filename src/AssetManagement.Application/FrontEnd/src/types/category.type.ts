export interface ICategory {
  id: string;
  name: string;
  prefix: string;
}

export interface ICreateCategoryRequest {
  name: string;
  prefix: string;
}
