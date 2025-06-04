export interface IAssetReport {
  category: string;
  total: number;
  assigned: number;
  available: number;
  notAvailable: number;
  waitingForRecycling: number;
  recycled: number;
}

export interface IAssetReportParams {
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}