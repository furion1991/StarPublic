'use client'

import { Button, PriceWithCurrency } from '@/shared/ui'

import { useSellItems } from '../../model/useSellItems'

type SellItemButtonProps = {
  inventoryItemId: string
  sellPrice: number
  className?: string
}

export const SellItemButton = ({ inventoryItemId, sellPrice, className }: SellItemButtonProps) => {
  const {
    mutate: sellItem,
    isSuccess: isItemSold,
    isPending: isItemSellInProccess
  } = useSellItems()

  return (
    <Button
      className={className}
      disabled={isItemSold}
      loading={isItemSellInProccess}
      onClick={() => {
        sellItem([inventoryItemId])
      }}
      color='purple'
    >
      {!isItemSold ? (
        <>
          Продать за
          <PriceWithCurrency>{sellPrice}</PriceWithCurrency>
        </>
      ) : (
        'Продано'
      )}
    </Button>
  )
}
