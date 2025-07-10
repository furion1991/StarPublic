import { LootRarity } from '@/entities/loot'

export type OpenCaseProps = {
  userId: string
  caseId: string
}

type CaseLootItem = {
  inventoryRecordId: string
  isItemActive: boolean
  itemState: number
  item: {
    id: string
    name: string
    type: number
    rarity: LootRarity
    baseCost: number
    sellPrice: number
    isVisible: boolean
    game: string
    image: string
    isAvailableForContract: boolean
  }
}

type CaseOpenResultData = {
  userId: string
  items: CaseLootItem[]
}

export type OpenedCaseData = {
  statusCode: number
  message: string
  result: CaseOpenResultData
}

export type OpenFewCasesProps = {
  userId: string
  caseId: string
  quantity: number
}
