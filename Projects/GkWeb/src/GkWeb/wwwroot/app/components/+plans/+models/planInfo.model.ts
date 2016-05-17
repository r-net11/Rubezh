import { PlanElement } from './planElement.model';

export class PlanInfo
{
	Description: string;
	Elements: Array<PlanElement>;
	Height: number;
	Name: string;
	NestedPlans: Array<PlanInfo>;
	ParentUid: string;
	Uid: string;
	Width: number;
	IsFolder: boolean;
}