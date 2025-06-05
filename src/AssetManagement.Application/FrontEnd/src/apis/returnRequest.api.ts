import http from "@/utils/http";

const returnRequestApi = {
  createReturnRequest: async (assignmentId: string) => {
    const response = await http.post("/returnrequests", { assignmentId });
    return response.data;
  },
};

export default returnRequestApi;
