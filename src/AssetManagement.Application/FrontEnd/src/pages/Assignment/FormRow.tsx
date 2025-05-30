/**
 * FormRow: Component hiển thị 1 dòng label + input + error
 * @param label - Nhãn trường
 * @param error - Thông báo lỗi (nếu có)
 * @param children - Input control
 */
const FormRow: React.FC<{ label: string; error?: string; children: React.ReactNode }> = ({ label, error, children }) => (
  <div className="grid grid-cols-12 items-center">
    <label className="col-span-4 font-medium text-gray-700">{label}</label>
    <div className="col-span-8">
      {children}
      {error && <p className="mt-1 text-sm text-red-600">{error}</p>}
    </div>
  </div>
);

export default FormRow;
