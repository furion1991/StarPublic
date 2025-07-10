import { LootRarity } from '@/entities/loot'

type LastOpenedCaseItemData = {
  baseCost: number
  game: string
  id: string
  image: string
  isVisible: boolean
  name: string
  rarity: LootRarity
  sellPrice: number
  type: number
}

export type LastOpenedCase = {
  caseId: string
  item: LastOpenedCaseItemData
  openedTimeStamp: string
  sellPrice: number | null
  userId: string
}
