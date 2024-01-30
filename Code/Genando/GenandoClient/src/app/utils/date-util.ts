import { formatDate } from "@angular/common";

export function setInputDate(date: Date): string
{
    return formatDate(date, 'yyyy-MM-dd', 'en');
}
