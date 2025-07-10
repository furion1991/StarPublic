'use client'

import { useEffect, useRef, useState } from 'react'
import { useMutation } from '@tanstack/react-query'

import { PageActions } from '@/shared/ui'
import { ContractItemsFillPanel, useContractAnimation } from '@/entities/contracts'
import { ContractResult, ItemsForContractList } from '@/widgets/contracts'
import { createContract, getContractItemApproximateValue } from '@/features/contracts'

import { useAuth, useShakingAnimation, useUser } from '@/shared/hooks'

import classes from './Contracts.module.scss'

type ItemIds = {
  itemId: string
  inventoryItemId: string
}

export const ContractsPage = () => {
  const MIN_ITEMS_NUMBER_IN_CONTRACT = 3
  const MAX_ITEMS_NUMBER_IN_CONTRACT = 10

  const { user } = useUser()
  const { isAuth } = useAuth()
  const [itemsIdsInContract, setItemsIdsInContract] = useState<ItemIds[]>([])
  const [isContractFinished, setContractFinished] = useState(false)

  const linkBackRef = useRef<HTMLButtonElement>(null)
  const soundBtnRef = useRef<HTMLButtonElement>(null)
  const headingRef = useRef<HTMLHeadingElement>(null)
  const fillPanelRef = useRef<HTMLDivElement>(null)
  const itemsForContractRef = useRef<HTMLDivElement>(null)

  const { isAnimationInProcess, isSparksActive, startAnimation } = useContractAnimation()

  useShakingAnimation({
    elementsRef: [linkBackRef, soundBtnRef, headingRef, fillPanelRef, itemsForContractRef],
    enabled: isAnimationInProcess
  })

  const {
    data: prizeItem,
    mutate: makeContract,
    isPending: isContractInProcess
  } = useMutation({
    mutationFn: createContract,
    onSuccess: async () => {
      startAnimation()
    }
  })

  const {
    data: itemValues,
    mutate: getItemValues,
    isPending: isItemValueLoading
  } = useMutation({
    mutationFn: getContractItemApproximateValue
  })

  useEffect(() => {
    if (!isAnimationInProcess && prizeItem) {
      setContractFinished(true)
    }
  }, [prizeItem, isAnimationInProcess])

  useEffect(() => {
    if (itemsIdsInContract.length < MIN_ITEMS_NUMBER_IN_CONTRACT) return

    getItemValues(itemsIdsInContract.map(({ itemId }) => itemId))
  }, [itemsIdsInContract])

  return (
    <>
      <PageActions linkBackRef={linkBackRef} soundBtnRef={soundBtnRef} />

      <div className={classes.contractsPage}>
        <div className={classes.wrapper}>
          <h1 ref={headingRef}>{!isContractFinished ? 'Контракты' : 'Поздравляем!'}</h1>

          {!isContractFinished ? (
            <>
              <div ref={fillPanelRef} className={classes.fillPanel}>
                <ContractItemsFillPanel
                  itemValueStart={itemValues?.minValue}
                  itemValueEnd={itemValues?.maxValue}
                  isContractInProcess={isContractInProcess}
                  isItemValueLoading={isItemValueLoading}
                  itemsInContractNumber={itemsIdsInContract.length}
                  isSparksActive={isSparksActive}
                  glowing={isAnimationInProcess}
                  onContractSubmit={() => {
                    if (!user) return

                    makeContract({
                      inventoryItemsIds: itemsIdsInContract.map(
                        ({ inventoryItemId }) => inventoryItemId
                      ),
                      userId: user.id
                    })
                  }}
                />
              </div>

              {isAuth ? (
                <div ref={itemsForContractRef} className={classes.itemsForContract}>
                  <div className={classes.label}>
                    <p>Доступные для контрактов предметы</p>
                  </div>

                  <div className={classes.itemsList}>
                    <ItemsForContractList
                      itemsIdsInContact={itemsIdsInContract}
                      onItemIdAddToContract={(itemIds) => {
                        if (itemsIdsInContract.length < MAX_ITEMS_NUMBER_IN_CONTRACT) {
                          setItemsIdsInContract([...itemsIdsInContract, itemIds])
                        }
                      }}
                      onItemIdRemoveFromContract={(itemIds) => {
                        setItemsIdsInContract(
                          itemsIdsInContract.filter(
                            ({ inventoryItemId, itemId }) =>
                              itemId !== itemIds.itemId && inventoryItemId !== itemIds.itemId
                          )
                        )
                      }}
                    />
                  </div>
                </div>
              ) : null}
            </>
          ) : null}

          {isContractFinished && prizeItem ? (
            <ContractResult
              prizeItem={prizeItem}
              onContractRestart={() => {
                setContractFinished(false)
              }}
              onItemAddToContract={(itemIds) => {
                setItemsIdsInContract([itemIds])
              }}
            />
          ) : null}
        </div>
      </div>
    </>
  )
}
