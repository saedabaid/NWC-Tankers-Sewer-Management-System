import { AlertType } from '../../Enums/alertType.enum';

export interface alert {
        type: AlertType;
        message: string;
        timeout: number;
}
