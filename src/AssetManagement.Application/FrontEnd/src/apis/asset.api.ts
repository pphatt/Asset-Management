import { IAsset, IAssetCategory, IAssetParams, IAssetState } from '@/types/asset.type';
import http from '../utils/http';

const assetApi = {
    getAssets: async (params: IAssetParams): Promise<HttpResponse<PaginatedResult<IAsset>>> => {
        const { data } = await http.get('/asset', { params });
        return data;
    },
    getAssetCategories: async (): Promise<HttpResponse<IAssetCategory[]>> => {
        const { data } = await http.get('/asset/categories');
        return data;
    },
    getAssetStates: async (): Promise<HttpResponse<IAssetState[]>> => {
        const { data } = await http.get('/asset/states');
        return data;
    },
};

export default assetApi;
