export const formatDate = (date: Date): string => {
  return date.toISOString();
};

// Adding new helper functions
export const formatDateToString = (date: Date): string => {
  // Kiểm tra nếu date không hợp lệ, trả về chuỗi rỗng
  if (!(date instanceof Date) || isNaN(date.getTime())) {
    console.warn('Invalid date provided to formatDateToString:', date);
    return '';
  }

  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
};

// Hàm chuyển đổi chuỗi ngày thành định dạng yyyy-MM-dd
export const parseAndFormatDate = (dateString: string): string => {
  if (!dateString) return '';

  // Nếu đã đúng định dạng yyyy-MM-dd rồi
  if (/^\d{4}-\d{2}-\d{2}$/.test(dateString)) {
    return dateString;
  }

  // Nếu là dd/MM/yyyy
  if (/^\d{2}\/\d{2}\/\d{4}$/.test(dateString)) {
    const [day, month, year] = dateString.split('/');
    return `${year}-${month}-${day}`;
  }

  // Thử parse bằng Date constructor
  const date = new Date(dateString);
  if (!isNaN(date.getTime())) {
    return formatDateToString(date);
  }

  console.warn('Could not parse date string:', dateString);
  return '';
};

export const getStartOfToday = (): Date => {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return today;
};

export const convertToInputFormat = (date: string): string => {
  const [day, month, year] = date.split('/');
  return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
}