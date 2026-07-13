export interface TreeNode {
    id: string;
    name: string;
    children: TreeNode[];
    isExpanded?:boolean;
    level: number;
    color: string;
    btnName: string;
  }